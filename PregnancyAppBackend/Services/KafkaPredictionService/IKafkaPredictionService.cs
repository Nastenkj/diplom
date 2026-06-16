using PregnancyAppBackend.Dtos.Web;

namespace PregnancyAppBackend.Services.KafkaPredictionService;

public interface IKafkaPredictionService
{
    /// <summary>
    /// Отправляет запрос на предсказание и ожидает ответ
    /// </summary>
    Task<HealthPredictionResponseDto?> GetPredictionAsync(HealthPredictionRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправляет запрос на предсказание в Kafka без ожидания ответа
    /// </summary>
    Task SendPredictionRequestAsync(HealthPredictionRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет доступность сервиса модели
    /// </summary>
    Task<bool> IsModelServiceAvailableAsync();
}

