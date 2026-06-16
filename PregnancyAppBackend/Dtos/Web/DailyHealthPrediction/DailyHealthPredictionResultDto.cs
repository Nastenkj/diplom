namespace PregnancyAppBackend.Dtos.Web.DailyHealthPrediction;

public class DailyHealthPredictionResultDto
{
    public Guid DailySurveyId { get; set; }
    public DateTime CreationDateUtc { get; set; }

    public int Trimester { get; set; }
    public int Prediction { get; set; }
    public string PredictionText { get; set; } = string.Empty;

    public PredictionProbabilitiesDto Probabilities { get; set; } = new();
    public List<DailyHealthPredictionDeviationDto> Deviations { get; set; } = new();
}

public class PredictionProbabilitiesDto
{
    public double Normal { get; set; }
    public double Alert { get; set; }
    public double Pathology { get; set; }
}

public class DailyHealthPredictionDeviationDto
{
    public string Feature { get; set; } = string.Empty;
    public decimal Value { get; set; }

    public string NormalRange { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
