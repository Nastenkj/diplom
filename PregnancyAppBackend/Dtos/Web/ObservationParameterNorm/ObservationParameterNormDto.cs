namespace PregnancyAppBackend.Dtos.Web.ObservationParameterNorm;

public class ObservationParameterNormDto
{
    public string ParameterName { get; set; }
    
    public decimal? LowerBound { get; set; }
    public decimal? UpperBound { get; set; }

    public Guid UserId { get; set; }
    public Guid ParameterId { get; set; }
}