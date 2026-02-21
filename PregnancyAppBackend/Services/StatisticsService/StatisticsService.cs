using Microsoft.EntityFrameworkCore;
using PregnancyAppBackend.Dtos.Web.Statistics;
using PregnancyAppBackend.Dtos.Web.Statistics.Date;
using PregnancyAppBackend.Entities;
using PregnancyAppBackend.Persistance;
using PregnancyAppBackend.Services.ObservationParameterNormService;
using PregnancyAppBackend.Services.UserInfoService;

namespace PregnancyAppBackend.Services.StatisticsService;

public class StatisticsService : IStatisticsService
{
    private readonly IDatabaseContext _databaseContext;
    private readonly IObservationParameterNormService _observationParameterNormService;
    private readonly IUserInfoService _userInfoService;
    private readonly ILogger<StatisticsService> _logger;

    public StatisticsService(IDatabaseContext databaseContext, 
                             IObservationParameterNormService observationParameterNormService,
                             IUserInfoService userInfoService,
                             ILogger<StatisticsService> logger)
    {
        _databaseContext = databaseContext;
        _observationParameterNormService = observationParameterNormService;
        _userInfoService = userInfoService;
        _logger = logger;
    }

    public async Task<StatisticsDatePlotResultDto> GetStatisticByDateAsync(string dbFieldName, DateTime? startDateUtc, DateTime? endDateUtc, Guid patientId, IDatabaseContext? databaseContext = null)
    {
        databaseContext ??= _databaseContext;
        
        var dailySurveyProperty = typeof(DailySurvey).GetProperty(dbFieldName);
        var weeklySurveyProperty = typeof(WeeklySurvey).GetProperty(dbFieldName);
    
        // if (dailySurveyProperty == null && weeklySurveyProperty == null)
        // {
        //     throw new ArgumentException($"Field {dbFieldName} does not exist in either DailySurvey or WeeklySurvey");
        // }
        //
        var plotPoints = new List<DatePlotPointDto>();
    
        if (dailySurveyProperty != null)
        {
            var dailyQuery = databaseContext.DailySurveys.Where(s => (startDateUtc == null || s.CreationDateUtc >= startDateUtc) && 
                                                                     (endDateUtc == null || s.CreationDateUtc <= endDateUtc) && 
                                                                     s.UserId == patientId);
    
            var dailySurveys = await dailyQuery.ToListAsync();
    
            var dailyPlotPoints = dailySurveys
                .Select(s => new { s.CreationDateUtc, Value = dailySurveyProperty.GetValue(s) })
                .Where(x => x.Value != null)
                .Select(x => new DatePlotPointDto
                {
                    FixationDateUtc = x.CreationDateUtc,
                    Value = Convert.ToDecimal(x.Value)
                });
                
            plotPoints.AddRange(dailyPlotPoints);
        }
    
        if (weeklySurveyProperty != null)
        {
            var weeklyQuery = databaseContext.WeeklySurveys.Where(s => (startDateUtc == null || s.CreationDateUtc >= startDateUtc) && 
                                                                       (endDateUtc == null || s.CreationDateUtc <= endDateUtc) && 
                                                                       s.UserId == patientId);
    
            var weeklySurveys = await weeklyQuery.ToListAsync();
    
            var weeklyPlotPoints = weeklySurveys
                .Select(s => new { s.CreationDateUtc, Value = weeklySurveyProperty.GetValue(s) })
                .Where(x => x.Value != null)
                .Select(x => new DatePlotPointDto
                {
                    FixationDateUtc = x.CreationDateUtc,
                    Value = Convert.ToDecimal(x.Value)
                });
                
            plotPoints.AddRange(weeklyPlotPoints);
        }
    
        plotPoints = plotPoints.OrderBy(v => v.FixationDateUtc).ToList();
    
        return new StatisticsDatePlotResultDto
        {
            PlotPoints = plotPoints
        };
    }

    public async Task<List<ObservationParameterWithNormDto>> GetObservationParametersStatistics(Guid userId)
    {
        var doctorId = _userInfoService.GetUserInfoFromToken().UserId;
        
        var norms = await _observationParameterNormService.GetObservationParameterNormsAsync(doctorId);
        var paramValues = await _databaseContext.AlgorithmicAnalysisParameterValues.Where(apv => apv.UserId == userId).ToListAsync();

        List<ObservationParameterWithNormDto> result = new();
        foreach (var norm in norms)
        {
            var statistic = paramValues.SingleOrDefault(pv => pv.ParameterId == norm.ParameterId);

            if (statistic is null)
            {
                _logger.LogWarning($"Not found statistic with paramName={norm.ParameterName}, userId={userId}");
                continue;
            }

            result.Add(new ObservationParameterWithNormDto
            {
                LowerBound = norm.LowerBound,
                UpperBound = norm.UpperBound,
                ParameterName = norm.ParameterName,
                Value = statistic.Value
            });
        }
        
        return result;
    }
}