using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PregnancyAppBackend.Dtos.Web.DailyHealthPrediction;
using PregnancyAppBackend.Services.HealthPredictionsService;
using PregnancyAppBackend.Services.UserInfoService;

namespace PregnancyAppBackend.Controllers.Web;

[Route("web/[controller]")]
[ApiController]
[Authorize]
public class DailyHealthPredictionsController : ControllerBase
{
    private readonly IHealthPredictionsService _healthPredictionsService;
    private readonly IUserInfoService _userInfoService;

    public DailyHealthPredictionsController(
        IHealthPredictionsService healthPredictionsService,
        IUserInfoService userInfoService)
    {
        _healthPredictionsService = healthPredictionsService;
        _userInfoService = userInfoService;
    }

    /// <summary>
    /// Просмотр результатов ML по ежедневным опросам пользователя (по датам)
    /// </summary>
    [HttpGet("results")]
    public async Task<ActionResult<List<DailyHealthPredictionResultDto>>> GetDailyResults(
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc)
    {
        var userId = _userInfoService.GetUserInfoFromToken().UserId;

        var results = await _healthPredictionsService.GetDailyResultsForUserAsync(userId, fromUtc, toUtc);
        return Ok(results);
    }

    /// <summary>
    /// Просмотр результата ML по конкретному ежедневному опросу пользователя
    /// </summary>
    [HttpGet("by-daily-survey")]
    public async Task<ActionResult<DailyHealthPredictionResultDto?>> GetDailyResultByDailySurveyId(
        [FromQuery] Guid dailySurveyId)
    {
        var userId = _userInfoService.GetUserInfoFromToken().UserId;

        var result = await _healthPredictionsService.GetDailyResultByDailySurveyIdAsync(userId, dailySurveyId);
        return Ok(result);
    }
}
