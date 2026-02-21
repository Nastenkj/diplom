using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PregnancyAppBackend.Dtos.Web.WeeklySurvey;
using PregnancyAppBackend.Exceptions;
using PregnancyAppBackend.Infrastructure.Security;
using PregnancyAppBackend.Services.UserInfoService;
using PregnancyAppBackend.Services.WeeklySurveyService;

namespace PregnancyAppBackend.Controllers.Web;

public class WeeklySurveysController : WebController
{
    private readonly IWeeklySurveysService _weeklySurveysService;
    private readonly IUserInfoService _userInfoService;

    public WeeklySurveysController(IWeeklySurveysService weeklySurveysService,
                                   IUserInfoService userInfoService)
    {
        _weeklySurveysService = weeklySurveysService;
        _userInfoService = userInfoService;
    }

    [HttpPost]
    [Authorize(Policy = Policies.PostWeeklySurvey)]

    public async Task<ActionResult<WeeklySurveyDto>> PostWeeklySurvey(WeeklySurveyDto weeklySurveyDto)
    {
        if (!ModelState.IsValid)
        {
            throw new ApiException(GetErrorListFromModelState(ModelState), "Произошла ошибка запроса, попробуйте позже.");
        }

        return await _weeklySurveysService.AddWeeklySurvey(weeklySurveyDto);
    }

    [HttpGet("latest-date")]
    [Authorize(Policy = Policies.GetLatestWeeklySurvey)]
    public async Task<ActionResult<DateTime?>> GetLatestWeeklySurvey()
    {
        return await _weeklySurveysService.GetLatestWeeklySurveyDateAsync();
    }
    
    [HttpGet]
    [Authorize(Policy = Policies.GetWeeklySurveysForUser)]
    public async Task<ActionResult<List<WeeklySurveyDto>>> GetWeeklySurveysForUser(Guid? userId)
    {
        if (userId is not null)
        {
            await _userInfoService.EnsureUserHasAccessToPatientInfoAsync();
        }
        else
        {
            userId = _userInfoService.GetUserInfoFromToken().UserId;
        }
        
        return await _weeklySurveysService.GetWeeklySurveysForUser(userId.Value);
    }
    
    [HttpGet("{surveyId}")]
    [Authorize(Policy = Policies.GetWeeklySurveyById)]
    public async Task<ActionResult<WeeklySurveyDto>> GetWeeklySurveyById(Guid surveyId)
    {
        return await _weeklySurveysService.GetWeeklySurveyById(surveyId);
    }
}