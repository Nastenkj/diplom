using PregnancyAppBackend.Entities.Security;

namespace PregnancyAppBackend.Entities;

public class HealthPredictionResult : Entity
{
    public Guid UserId { get; set; }
    public Guid DailySurveyId { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public int Trimester { get; set; }
    public int Prediction { get; set; }
    public string PredictionText { get; set; } = string.Empty;

    public double NormalProbability { get; set; }
    public double AlertProbability { get; set; }
    public double PathologyProbability { get; set; }

    public List<HealthPredictionDeviation> Deviations { get; set; } = new();
}

public class HealthPredictionDeviation
{
    public int Id { get; set; }

    public string Feature { get; set; } = string.Empty;
    public decimal Value { get; set; }

    public string NormalRange { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public Guid HealthPredictionResultId { get; set; }
    public HealthPredictionResult? HealthPredictionResult { get; set; }
}
