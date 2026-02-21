using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PregnancyAppBackend.Dtos.Web.DailySurvey;
using PregnancyAppBackend.Exceptions;
using PregnancyAppBackend.Infrastructure.Security;
using PregnancyAppBackend.Services.DailySurveysService;
using PregnancyAppBackend.Services.UserInfoService;

namespace PregnancyAppBackend.Controllers.Web;

public class DailySurveysController : WebController
{
    private readonly IDailySurveysService _dailySurveysService;
    private readonly IUserInfoService _userInfoService;

    public DailySurveysController(IDailySurveysService dailySurveysService,
                                  IUserInfoService userInfoService)
    {
        _dailySurveysService = dailySurveysService;
        _userInfoService = userInfoService;
    }

    [HttpPost]
    [Authorize(Policy = Policies.PostDailySurvey)]
    public async Task<ActionResult<DailySurveyDto>> PostDailySurvey(DailySurveyDto dailySurveyDto)
    {
        if (!ModelState.IsValid)
        {
            throw new ApiException(GetErrorListFromModelState(ModelState), "Произошла ошибка запроса, попробуйте позже.");
        }

        return await _dailySurveysService.AddDailySurvey(dailySurveyDto);
    }

    [HttpGet("latest-date")]
    [Authorize(Policy = Policies.GetLatestDailySurveyCreationDateUtc)]
    public async Task<ActionResult<DateTime?>> GetLatestDailySurveyCreationDateUtc()
    {
        return await _dailySurveysService.GetLatestDailySurveyCreationDateUtcAsync();
    }
    
    [HttpGet]
    [Authorize(Policy = Policies.GetDailySurveysForUser)]
    public async Task<ActionResult<List<DailySurveyDto>>> GetDailySurveysForUser(Guid? userId)
    {
        if (userId is not null)
        {
            await _userInfoService.EnsureUserHasAccessToPatientInfoAsync();
        }
        else
        {
            userId = _userInfoService.GetUserInfoFromToken().UserId;
        }

        return await _dailySurveysService.GetDailySurveysForUser(userId.Value);
    }
    
    [HttpGet("{surveyId}")]
    [Authorize(Policy = Policies.GetDailySurveyById)]
    public async Task<ActionResult<DailySurveyDto>> GetDailySurveyById(Guid surveyId)
    {
        return await _dailySurveysService.GetDailySurveyById(surveyId);
    }
}