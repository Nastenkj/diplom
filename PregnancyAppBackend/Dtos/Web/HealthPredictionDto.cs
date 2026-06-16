using System.Text.Json.Serialization;

namespace PregnancyAppBackend.Dtos.Web;

public class HealthPredictionRequestDto
{
    /// <summary>
    /// 21 признак в порядке модели
    /// </summary>
    [JsonPropertyName("features")]
    public List<double> Features { get; set; } = new();

    /// <summary>
    /// Триместр беременности: 1, 2 или 3
    /// </summary>
    [JsonPropertyName("trimester")]
    public int Trimester { get; set; }

    /// <summary>
    /// ID пользователя (опционально)
    /// </summary>
    [JsonPropertyName("user_id")]
    public Guid? UserId { get; set; }

    /// <summary>
    /// ID ежедневного обследования (опционально)
    /// </summary>
    [JsonPropertyName("daily_survey_id")]
    public Guid? DailySurveyId { get; set; }
}

public class HealthPredictionResponseDto
{
    /// <summary>
    /// Предсказание: 0=Норма, 1=Предупреждение, 2=Патология
    /// </summary>
    [JsonPropertyName("prediction")]
    public int Prediction { get; set; }

    /// <summary>
    /// Текстовое описание результата
    /// </summary>
    [JsonPropertyName("prediction_text")]
    public string PredictionText { get; set; } = string.Empty;

    /// <summary>
    /// Вероятности по классам
    /// </summary>
    [JsonPropertyName("probabilities")]
    public PredictionProbabilities Probabilities { get; set; } = new();

    /// <summary>
    /// Описание девиаций / проблемных показателей
    /// </summary>
    [JsonPropertyName("deviations")]
    public List<HealthPredictionDeviationDto> Deviations { get; set; } = new();

    /// <summary>
    /// ID запроса (для отслеживания)
    /// </summary>
    [JsonPropertyName("RequestId")]
    public string RequestId { get; set; } = string.Empty;

    /// <summary>
    /// Триместр, для которого делалось предсказание
    /// </summary>
    [JsonPropertyName("trimester")]
    public int Trimester { get; set; } = 0;
}

public class PredictionProbabilities
{
    [JsonPropertyName("normal")]
    public double Normal { get; set; }
    
    [JsonPropertyName("alert")]
    public double Alert { get; set; }

    [JsonPropertyName("patology")]
    public double Pathology { get; set; }
}

/// <summary>
/// Kafka message для отправки запроса на предсказание
/// </summary>
public class PredictionRequestMessage
{
    public string RequestId { get; set; } = Guid.NewGuid().ToString();
    public List<double> Features { get; set; } = new();
    public int Trimester { get; set; }

    // Чтобы backend мог однозначно связать response ML с daily survey и пользователем
    [JsonPropertyName("user_id")]
    public Guid? UserId { get; set; }

    [JsonPropertyName("daily_survey_id")]
    public Guid? DailySurveyId { get; set; }
}

/// <summary>
/// Kafka message для получения ответа предсказания
/// </summary>
public class HealthPredictionDeviationDto
{
    [JsonPropertyName("feature")]
    public string Feature { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    [JsonPropertyName("normal_range")]
    public string NormalRange { get; set; } = string.Empty;

    [JsonPropertyName("severity")]
    public string Severity { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

public class PredictionResponseMessage
{
    [JsonPropertyName("RequestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("prediction")]
    public int Prediction { get; set; }

    [JsonPropertyName("prediction_text")]
    public string PredictionText { get; set; } = string.Empty;

    [JsonPropertyName("probabilities")]
    public PredictionProbabilities Probabilities { get; set; } = new();

    [JsonPropertyName("trimester")]
    public int Trimester { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("deviations")]
    public List<HealthPredictionDeviationDto>? Deviations { get; set; }

    // В ответе ML обязаны быть идентификаторы для сохранения в БД и уведомления пользователю
    [JsonPropertyName("user_id")]
    public Guid? UserId { get; set; }

    [JsonPropertyName("daily_survey_id")]
    public Guid? DailySurveyId { get; set; }

    [JsonPropertyName("created_at_utc")]
    public DateTime? CreatedAtUtc { get; set; }
}

