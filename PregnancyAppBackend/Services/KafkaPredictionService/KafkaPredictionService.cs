using System.Collections.Concurrent;
using System.Text.Json;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PregnancyAppBackend.Dtos.Web;
using PregnancyAppBackend.Services.HealthPredictionsService;

namespace PregnancyAppBackend.Services.KafkaPredictionService;

public class KafkaPredictionService : IKafkaPredictionService, IDisposable
{
    private readonly ILogger<KafkaPredictionService> _logger;
    private readonly KafkaSettings _settings;
    private readonly IProducer<string, string> _producer;
    private readonly IConsumer<string, string> _consumer;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<HealthPredictionResponseDto>> _pendingRequests = new();
    private const string RequestTopic = "prediction-requests";
    private const string ResponseTopic = "prediction-responses";
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _consumerTask;

    private readonly IHealthPredictionsService _healthPredictionsService;

    public KafkaPredictionService(
        IOptions<KafkaSettings> settings,
        ILogger<KafkaPredictionService> logger,
        IHealthPredictionsService healthPredictionsService)
    {
        _logger = logger;
        _settings = settings.Value;
        _healthPredictionsService = healthPredictionsService;

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            Acks = Acks.Leader,
            LingerMs = 10,
            EnableIdempotence = false
        };

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = "prediction-client-group",
            AutoOffsetReset = AutoOffsetReset.Latest,
            EnableAutoCommit = true,
            EnableAutoOffsetStore = true
        };

        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        _consumer.Subscribe(ResponseTopic);

        // Запускаем фоновый консьюмер для обработки ответов
        _consumerTask = Task.Run(ProcessResponsesAsync, _cts.Token);
        
        _logger.LogInformation("Kafka prediction service initialized with bootstrap servers: {BootstrapServers}", _settings.BootstrapServers);
    }

    private async Task ProcessResponsesAsync()
    {
        try
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    var message = _consumer.Consume(TimeSpan.FromSeconds(1));
                    if (message == null || message.Message == null)
                        continue;

                    var responseJson = message.Message.Value;
                    var response = JsonSerializer.Deserialize<PredictionResponseMessage>(responseJson);

                    if (response == null)
                        continue;

                    // Основная логика: сохраняем ML результат в БД по факту получения ответа.
                    // Это не зависит от HTTP ожидания (send & forget).
                    if (string.IsNullOrEmpty(response.Error))
                    {
                        // Привязка результата ML к dailySurvey возможна только если пришли идентификаторы.
                        if (response.UserId.HasValue && response.DailySurveyId.HasValue)
                        {
                            try
                            {
                                var deviations = response.Deviations?.Select(d => new HealthPredictionDeviationDto
                                {
                                    Feature = d.Feature,
                                    Value = d.Value,
                                    NormalRange = d.NormalRange,
                                    Severity = d.Severity,
                                    Message = d.Message
                                }).ToList() ?? new List<HealthPredictionDeviationDto>();

                                _logger.LogInformation(
                                    "Saving ML prediction to DB. RequestId={RequestId}, UserId={UserId}, DailySurveyId={DailySurveyId}, DeviationsCount={DeviationsCount}",
                                    response.RequestId, response.UserId, response.DailySurveyId, deviations.Count);

                                await _healthPredictionsService.SavePredictionForDailySurveyAsync(
                                    userId: response.UserId.Value,
                                    dailySurveyId: response.DailySurveyId.Value,
                                    createdAtUtc: response.CreatedAtUtc ?? DateTime.UtcNow,
                                    trimester: response.Trimester,
                                    prediction: response.Prediction,
                                    predictionText: response.PredictionText,
                                    normalProbability: response.Probabilities?.Normal ?? 0,
                                    alertProbability: response.Probabilities?.Alert ?? 0,
                                    pathologyProbability: response.Probabilities?.Pathology ?? 0,
                                    deviations: deviations
                                );
                            }
                            catch (Exception saveEx)
                            {
                                _logger.LogError(saveEx,
                                    "Failed to save ML prediction to DB. RequestId={RequestId}, UserId={UserId}, DailySurveyId={DailySurveyId}",
                                    response.RequestId, response.UserId, response.DailySurveyId);
                            }
                        }
                        else
                        {
                            _logger.LogError(
                                "ML response missing UserId/DailySurveyId. RequestId={RequestId}, UserId={UserId}, DailySurveyId={DailySurveyId}",
                                response.RequestId, response.UserId, response.DailySurveyId);
                        }
                    }
                    else
                    {
                        _logger.LogError("Prediction error: {Error}", response.Error);
                    }

                    // Запасной механизм: если где-то ещё остались ожидания (tcs),
                    // то завершаем их.
                    if (_pendingRequests.TryRemove(response.RequestId, out var tcs))
                    {
                        var dto = new HealthPredictionResponseDto
                        {
                            Prediction = response.Prediction,
                            PredictionText = response.PredictionText,
                            RequestId = response.RequestId,
                            Trimester = response.Trimester,
                            Probabilities = new PredictionProbabilities
                            {
                                Normal = response.Probabilities?.Normal ?? 0,
                                Alert = response.Probabilities?.Alert ?? 0,
                                Pathology = response.Probabilities?.Pathology ?? 0
                            },
                            Deviations = response.Deviations?.Select(d => new HealthPredictionDeviationDto
                            {
                                Feature = d.Feature,
                                Value = d.Value,
                                NormalRange = d.NormalRange,
                                Severity = d.Severity,
                                Message = d.Message
                            }).ToList() ?? new List<HealthPredictionDeviationDto>()
                        };

                        if (!string.IsNullOrEmpty(response.Error))
                        {
                            tcs.TrySetException(new Exception(response.Error));
                        }
                        else
                        {
                            tcs.TrySetResult(dto);
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogWarning(ex, "Kafka consume error");
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "JSON deserialization error");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in Kafka consumer");
        }
    }

    public async Task SendPredictionRequestAsync(HealthPredictionRequestDto request, CancellationToken cancellationToken = default)
    {
        var requestId = Guid.NewGuid().ToString();

        var message = new PredictionRequestMessage
        {
            RequestId = requestId,
            Features = request.Features,
            Trimester = request.Trimester,

            // Передаём идентификаторы, чтобы consumer смог сохранить результат в БД
            UserId = request.UserId,
            DailySurveyId = request.DailySurveyId
        };

        var json = JsonSerializer.Serialize(message);

        var kafkaMessage = new Message<string, string>
        {
            Key = requestId,
            Value = json
        };

        var deliveryResult = await _producer.ProduceAsync(RequestTopic, kafkaMessage, cancellationToken);

        if (deliveryResult.Status == PersistenceStatus.NotPersisted)
        {
            _logger.LogError("Message was not persisted to Kafka");
            return;
        }

        _logger.LogInformation("Sent prediction request (fire-and-forget) {RequestId}", requestId);
    }

    public async Task<HealthPredictionResponseDto?> GetPredictionAsync(HealthPredictionRequestDto request, CancellationToken cancellationToken = default)
    {
        var requestId = Guid.NewGuid().ToString();

        var message = new PredictionRequestMessage
        {
            RequestId = requestId,
            Features = request.Features,
            Trimester = request.Trimester,

            // Передаём идентификаторы, чтобы consumer смог сохранить результат в БД
            UserId = request.UserId,
            DailySurveyId = request.DailySurveyId
        };

        var json = JsonSerializer.Serialize(message);

        // Создаём TaskCompletionSource для ожидания ответа
        var tcs = new TaskCompletionSource<HealthPredictionResponseDto>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        _pendingRequests[requestId] = tcs;

        try
        {
            var kafkaMessage = new Message<string, string>
            {
                Key = requestId,
                Value = json
            };

            var deliveryResult = await _producer.ProduceAsync(RequestTopic, kafkaMessage, cancellationToken);

            if (deliveryResult.Status == PersistenceStatus.NotPersisted)
            {
                _logger.LogError("Message was not persisted to Kafka");
                _pendingRequests.TryRemove(requestId, out _);
                return null;
            }

            _logger.LogInformation("Sent prediction request {RequestId}", requestId);

            // Ожидаем ответ с таймаутом
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            try
            {
                var result = await tcs.Task.WaitAsync(linkedCts.Token);
                return result;
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("Prediction request {RequestId} timed out", requestId);
                _pendingRequests.TryRemove(requestId, out _);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending prediction request");
            _pendingRequests.TryRemove(requestId, out _);
            throw;
        }
    }

    public async Task<bool> IsModelServiceAvailableAsync()
    {
        try
        {
            // Проверяем доступность Kafka через AdminClient
            using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _settings.BootstrapServers }).Build();
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
            return metadata.Brokers.Count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Model service health check failed");
            return false;
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _consumerTask.Wait(TimeSpan.FromSeconds(5));
        _consumer.Dispose();
        _producer.Dispose();
        _cts.Dispose();
    }
}

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = "localhost:9092";
}

