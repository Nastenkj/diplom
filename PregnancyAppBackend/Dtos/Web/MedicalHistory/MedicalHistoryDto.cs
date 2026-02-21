using PregnancyAppBackend.Enums.MedicalHistory;

namespace PregnancyAppBackend.Dtos.Web.MedicalHistory;

public class MedicalHistoryDto
{
    public int Weight { get; set; }
    public int Height { get; set; }
    public BloodGroup BloodGroup { get; set; }
    public RhesusFactor RhesusFactor { get; set; }
    public string BloodPressure { get; set; } = null!;
    public Thermometer Thermometer { get; set; }
    public int PregnancyAmount { get; set; }
    public int? AbortionAmount { get; set; }
    public int? MiscarriageAmount { get; set; }
    public int? PrematureBirthAmount { get; set; }
    public BirthType PreviousBirthType { get; set; }
    public string? GynecologicalDiseases { get; set; }
    public string? SomaticDiseases { get; set; }
    public string? UndergoneOperations { get; set; }
    public string? AllergicReactions { get; set; }
    public List<HereditaryDisease> HereditaryDiseases { get; set; } = new();
    public bool IsSmoking { get; set; }
    public bool IsConsumingAlcohol { get; set; }
    public CovidStatus EnduredCovid { get; set; }
}