using Microsoft.EntityFrameworkCore;
using PregnancyAppBackend.Persistance;
using PregnancyAppBackend.Services.DailySurveysService;
using PregnancyAppBackend.Services.MedicalHistoriesService;
using PregnancyAppBackend.Services.ObservationParameterNormService;
using PregnancyAppBackend.Services.StatisticsService;
using PregnancyAppBackend.Services.WeeklySurveyService;

namespace PregnancyAppBackend.Services.AlgorithmicAnalysis;

public class AlgorithmicAnalysisService : IAlgorithmicAnalysisService
{
    private readonly IMedicalHistoriesService _medicalHistoriesService;
    private readonly IDailySurveysService _dailySurveysService;
    private readonly IWeeklySurveysService _weeklySurveysService;
    private readonly ILogger<AlgorithmicAnalysisService> _logger;
    private readonly IObservationParameterNormService _observationParameterNormService;
    private readonly IStatisticsService _statisticsService;
    private readonly IDatabaseContext _databaseContext;

    public AlgorithmicAnalysisService(IMedicalHistoriesService medicalHistoriesService,
                                      IDailySurveysService dailySurveysService,
                                      IWeeklySurveysService weeklySurveysService,
                                      ILogger<AlgorithmicAnalysisService> logger,
                                      IObservationParameterNormService observationParameterNormService,
                                      IStatisticsService statisticsService,
                                      IDatabaseContext databaseContext)
    {
        _medicalHistoriesService = medicalHistoriesService;
        _dailySurveysService = dailySurveysService;
        _weeklySurveysService = weeklySurveysService;
        _logger = logger;
        _observationParameterNormService = observationParameterNormService;
        _statisticsService = statisticsService;
        _databaseContext = databaseContext;
    }
    
    public async Task UpdateDbResultFromSurveysAsync(Guid userId, IDatabaseContext? databaseContext = null)
    {
        databaseContext ??= _databaseContext;
        
        // await Task.Delay(2000);
        
        var medicalHistoryDto = await _medicalHistoriesService.GetMedicalHistoryAsync(userId, databaseContext);

        if (medicalHistoryDto is null)
        {
            _logger.LogInformation($"No medical histories for user with id={userId}");
            return;
        }
        
        var weeklySurveyDtos = await _weeklySurveysService.GetWeeklySurveysForUser(userId, databaseContext);

        if (!weeklySurveyDtos.Any())
        {
            _logger.LogInformation($"No weekly surveys for user with id={userId}");
            return;
        }
        
        var dailySurveyDtos = await _dailySurveysService.GetDailySurveysForUser(userId, databaseContext);

        if (!dailySurveyDtos.Any())
        {
            _logger.LogInformation($"No daily surveys for user with id={userId}");
            return;
        }

        var norms = await _observationParameterNormService.GetCurrentObservationParameterNormsAsync(databaseContext);

        foreach (var norm in norms)
        {
                var statistic = await _statisticsService.GetStatisticByDateAsync(norm.ParameterName, null, null, userId, databaseContext);
                if (statistic.PlotPoints.Any())
                {
                    var existingValue = await databaseContext.AlgorithmicAnalysisParameterValues
                                                             .Where(v => v.UserId == userId && v.ParameterId == norm.ParameterId)
                                                             .SingleOrDefaultAsync();
                    if (existingValue is null)
                    {
                        databaseContext.AlgorithmicAnalysisParameterValues.Add(new() { 
                            ParameterId = norm.ParameterId,
                            Value = statistic.PlotPoints.Select(pp => pp.Value).Average(),
                            UserId = userId
                        });    
                    }
                    else
                    {
                        existingValue.Value = statistic.PlotPoints.Select(pp => pp.Value).Average();
                        existingValue.UpdatedAtUtc = DateTime.UtcNow;
                    }
                }
        }
        
        await databaseContext.SaveChangesAsync();
    }

    public Task<List<Entities.AlgorithmicAnalysisParameterValue>> GetAlgorithmicAnalysisAsync(Guid userId)
    {
        return _databaseContext.AlgorithmicAnalysisParameterValues.Where(apv => apv.UserId == userId).ToListAsync();
    }
}