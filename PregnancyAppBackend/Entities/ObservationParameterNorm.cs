using PregnancyAppBackend.Entities.Security;

namespace PregnancyAppBackend.Entities;

public class ObservationParameterNorm : Entity
{
    public Guid ParameterId { get; set; }
    public Parameter Parameter { get; set; }

    public decimal? LowerBound { get; set; }
    public decimal? UpperBound { get; set; }
    public Guid UserId { get; set; }
    
    // Doctor who set this norm
    public User User { get; set; }
    
}