using Microsoft.EntityFrameworkCore;
using PregnancyAppBackend.Converters;
using PregnancyAppBackend.Dtos.Web.ObservationParameterNorm;
using PregnancyAppBackend.Exceptions;
using PregnancyAppBackend.Persistance;

namespace PregnancyAppBackend.Services.ObservationParameterNormService;

public class ObservationParameterNormService : IObservationParameterNormService
{
    private readonly IDatabaseContext _databaseContext;
    private readonly ILogger<ObservationParameterNormService> _logger;

    public ObservationParameterNormService(IDatabaseContext databaseContext, ILogger<ObservationParameterNormService> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }
    
    public async Task<List<ObservationParameterNormDto>> GetObservationParameterNormsAsync(Guid userId)
    {
        var norms = await _databaseContext.ObservationParameterNorms
                                          .Include(pn => pn.Parameter)
                                          .Where(opn => opn.UserId == userId)
                                          .OrderBy(opn => opn.Parameter.ParameterName)
                                          .ToListAsync();

        return norms.Select(n => n.ConvertToDto()).ToList();
    }

    public async Task<List<ObservationParameterNormDto>> GetCurrentObservationParameterNormsAsync(IDatabaseContext? databaseContext = null)
    {
        databaseContext ??= _databaseContext;
        
        var firstUser = await databaseContext.ObservationParameterNorms
                                             .Include(pn => pn.Parameter)
                                             .OrderBy(opn => opn.UserId)
                                             .Select(opn => opn.UserId)
                                             .FirstOrDefaultAsync();

        if (firstUser == default)
        {
            throw new Exception($"No observation parameters found for user with id={firstUser}");
        }
    
        var norms = await databaseContext.ObservationParameterNorms
                                         .Include(pn => pn.Parameter)
                                         .Where(opn => opn.UserId == firstUser)
                                         .OrderBy(opn => opn.Parameter.ParameterName)
                                         .ToListAsync();

        return norms.Select(n => n.ConvertToDto()).ToList();
    }

    public async Task<ObservationParameterNormDto> UpdateObservationParameterNormBoundsAsync(Guid userId, 
                                                                                             decimal lowerBound, 
                                                                                             decimal upperBound, 
                                                                                             string parameterName)
    {
        _logger.LogInformation("Updating observation parameter norms for userId={userId}, parameterName={parameterName}, lowerBound={lowerBound}, upperBound={upperBound},", userId, parameterName, lowerBound, upperBound);
        
        var norm = await _databaseContext.ObservationParameterNorms
                                         .Include(pn => pn.Parameter)
                                         .Where(opn => opn.UserId == userId && opn.Parameter.ParameterName == parameterName)
                                         .SingleOrDefaultAsync();

        if (norm is null)
        {
            throw new ApiException($"Not found norms for userId={userId}, parameterName={parameterName}", "Произошла ошибка при запросе. Попробуйте позже.");
        }

        if (upperBound < lowerBound)
        {
            throw new ApiException($"Incorrect values for norms, upperBound={upperBound}, lowerBound={lowerBound}", 
                                   "Нижняя граница должна быть меньше или равна верхней границы.");
        }
        
        norm.UpperBound = upperBound;
        norm.LowerBound = lowerBound;

        await _databaseContext.SaveChangesAsync();

        return norm.ConvertToDto();
    }
}