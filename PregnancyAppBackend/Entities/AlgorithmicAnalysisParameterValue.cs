namespace PregnancyAppBackend.Entities;

public class AlgorithmicAnalysisParameterValue : Entity
{
    public decimal Value { get; set; }
    
    // todo сделать таблицу Parameters и сюда ParameterId вместо этого поля.
    // В ObservationParameterNorms то же самое.
    
    public Guid ParameterId { get; set; }
    public Parameter Parameter { get; set; }
    
    public DateTime UpdatedAtUtc {
        get;
        set;
    }

    public Guid UserId { get; set; }
}
