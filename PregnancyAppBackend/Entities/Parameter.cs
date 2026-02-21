namespace PregnancyAppBackend.Entities;

public class Parameter : Entity
{
    public string ParameterName { get; set; }
    public decimal? DefaultLowerBoundValue { get; set; }
    public decimal? DefaultUpperBoundValue { get; set; }
}