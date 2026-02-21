using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PregnancyAppBackend.Dtos.Web.Statistics;
using PregnancyAppBackend.Dtos.Web.Statistics.Date;
using PregnancyAppBackend.Infrastructure.Security;
using PregnancyAppBackend.Services.StatisticsService;
using PregnancyAppBackend.Services.UserInfoService;

namespace PregnancyAppBackend.Controllers.Web;

public class StatisticsController : WebController
{
    private readonly IStatisticsService _statisticsService;
    private readonly IUserInfoService _userInfoService;

    public StatisticsController(IStatisticsService statisticsService,
                                IUserInfoService userInfoService)
    {
        _statisticsService = statisticsService;
        _userInfoService = userInfoService;
    }
    
    [HttpGet]
    [Authorize(Policy = Policies.GetDoctor)]
    public async Task<ActionResult<StatisticsDatePlotResultDto>> Get(string dbFieldName, 
                                                                     DateTime? startDateUtc, 
                                                                     DateTime? endDateUtc, 
                                                                     Guid patientId)
    {
        await _userInfoService.EnsureUserHasAccessToPatientInfoAsync();
        
        return await _statisticsService.GetStatisticByDateAsync(dbFieldName, startDateUtc, endDateUtc, patientId);
    }
    
    [HttpGet("with-average-values")]
    [Authorize(Policy = Policies.GetDoctor)]
    public async Task<List<ObservationParameterWithNormDto>> GetObservationParametersStatistics(Guid patientId)
    {
        await _userInfoService.EnsureUserHasAccessToPatientInfoAsync();
        
        return await _statisticsService.GetObservationParametersStatistics(patientId);
    }
}