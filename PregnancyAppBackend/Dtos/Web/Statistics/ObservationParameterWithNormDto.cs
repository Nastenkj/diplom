using PregnancyAppBackend.Dtos.Web.ObservationParameterNorm;

namespace PregnancyAppBackend.Dtos.Web.Statistics;

public class ObservationParameterWithNormDto : ObservationParameterNormDto
{
    public decimal Value { get; set; }
}