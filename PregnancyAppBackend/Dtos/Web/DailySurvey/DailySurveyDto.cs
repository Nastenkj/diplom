using PregnancyAppBackend.Enums.DailySurvey;

namespace PregnancyAppBackend.Dtos.Web.DailySurvey;

public class DailySurveyDto
{
    public Guid Id { get; set; }
    public bool AbdomenHurts { get; set; }
    public bool BloodDischarge { get; set; }
    public bool Nausea { get; set; }
    public int UrgeToPuke { get; set; }
    public Temperature Temperature { get; set; }
    public int SystolicPressure { get; set; }
    public int DiastolicPressure { get; set; }
    public int HeartRate { get; set; }
    public decimal? GlucoseLevel { get; set; }
    public decimal? HemoglobinLevel { get; set; }
    public decimal? Saturation { get; set; }
    public decimal? Uro { get; set; }
    public decimal? Bld { get; set; }
    public decimal? Bil { get; set; }
    public decimal? Ket { get; set; }
    public decimal? Leu { get; set; }
    public decimal? Glu { get; set; }
    public decimal? Pro { get; set; }
    public decimal? Ph { get; set; }
    public decimal? Nit { get; set; }
    public decimal? Sg { get; set; }
    public decimal? Vc { get; set; }
    public decimal? Pt { get; set; }
    public decimal? Aptt { get; set; }
    public decimal? Inr { get; set; }
    public decimal? OxygenLevel { get; set; }
    public string? AdditionalInformation { get; set; }
    public DateTime? CreationDateUtc { get; set; }
}