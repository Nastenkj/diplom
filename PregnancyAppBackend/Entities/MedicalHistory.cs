using PregnancyAppBackend.Entities.Security;
using PregnancyAppBackend.Enums.MedicalHistory;
using System.ComponentModel.DataAnnotations.Schema;

namespace PregnancyAppBackend.Entities;

public class MedicalHistory : Entity
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
    
    private string _hereditaryDiseases = string.Empty;
    public bool IsSmoking { get; set; }
    public bool IsConsumingAlcohol { get; set; }
    public CovidStatus EnduredCovid { get; set; }
    public DateTime CreationDateUtc { get; set; }

    public Guid UserId { get; set; }
    
    [NotMapped]
    public List<HereditaryDisease> HereditaryDiseases 
    {
        get
        {
            if (string.IsNullOrEmpty(_hereditaryDiseases))
                return new List<HereditaryDisease>();
                
            return _hereditaryDiseases
                  .Split(';', StringSplitOptions.RemoveEmptyEntries)
                  .Select(s => Enum.TryParse<HereditaryDisease>(s, out var disease) ? disease : HereditaryDisease.None)
                  .ToList();
        }
        set
        {
            _hereditaryDiseases = string.Join(";", value.Select(d => (int)d));
        }
    }
    
    [Column("HereditaryDiseases")]
    public string HereditaryDiseasesString
    {
        get => _hereditaryDiseases;
        set => _hereditaryDiseases = value;
    }
    public User User { get; set; } = null!;
}

public interface IMedicalHistory
{
    public int Weight { get; set; }
    public int Height { get; set; }
    public BloodGroup BloodGroup { get; set; }
    public RhesusFactor RhesusFactor { get; set; }
    public string BloodPressure { get; set; }
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
    public bool IsSmoking { get; set; }
    public bool IsConsumingAlcohol { get; set; }
    public CovidStatus EnduredCovid { get; set; }
}