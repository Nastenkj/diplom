using PregnancyAppBackend.Enums.WeeklySurvey;

namespace PregnancyAppBackend.Dtos.Web.WeeklySurvey;

public class WeeklySurveyDto
{
    public Guid Id { get; set; }
    public bool HasOrvi { get; set; }
    public string? OrviSymptoms { get; set; }
    public bool HasUnordinaryTemp { get; set; }
    public int? UnordinaryTempOccurrences { get; set; }
    public BloodPressure UnordinaryBloodPressure { get; set; }
    public bool HasGynecologicalSymptoms { get; set; }
    public string? GynecologicalSymptoms { get; set; }
    public bool HasUnordinaryUrine { get; set; }
    public bool HasSwelling { get; set; }
    public string? SwellingDescription { get; set; }
    public WaterConsumed WaterConsumed { get; set; }
    public Stool Stool { get; set; }
    public Urination Urination { get; set; }
    public decimal WeightAdded { get; set; }
    public int PregnancyWeek { get; set; }
    public DateTime? CreationDateUtc { get; set; }
}

