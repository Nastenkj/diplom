using PregnancyAppBackend.Dtos.Web.ObservationParameterNorm;
using PregnancyAppBackend.Persistance;

namespace PregnancyAppBackend.Services.ObservationParameterNormService;

public interface IObservationParameterNormService
{
    Task<List<ObservationParameterNormDto>> GetObservationParameterNormsAsync(Guid userId);
    Task<List<ObservationParameterNormDto>> GetCurrentObservationParameterNormsAsync(IDatabaseContext? databaseContext = null);
    Task<ObservationParameterNormDto> UpdateObservationParameterNormBoundsAsync(Guid userId, 
                                                                                 decimal lowerBound, 
                                                                                 decimal upperBound, 
                                                                                 string parameterName);
}