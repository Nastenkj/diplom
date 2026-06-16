using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PregnancyAppBackend.Dtos.Web;
using PregnancyAppBackend.Dtos.Web.DailySurvey;
using PregnancyAppBackend.Services.DailySurveysService;
using PregnancyAppBackend.Services.KafkaPredictionService;
using PregnancyAppBackend.Services.WeeklySurveyService;

namespace PregnancyAppBackend.Controllers.Web;

[Route("web/[controller]")]
[ApiController]
[Authorize]
public class HealthPredictionsController : ControllerBase
{
    private readonly IKafkaPredictionService _kafkaPredictionService;
    private readonly IDailySurveysService _dailySurveysService;
    private readonly IWeeklySurveysService _weeklySurveysService;
    private readonly ILogger<HealthPredictionsController> _logger;

    public HealthPredictionsController(
        IKafkaPredictionService kafkaPredictionService,
        IDailySurveysService dailySurveysService,
        IWeeklySurveysService weeklySurveysService,
        ILogger<HealthPredictionsController> logger)
    {
        _kafkaPredictionService = kafkaPredictionService;
        _dailySurveysService = dailySurveysService;
        _weeklySurveysService = weeklySurveysService;
        _logger = logger;
    }

    /// <summary>
    /// Получить прогноз здоровья на основе последнего ежедневного обследования
    /// </summary>
    [HttpGet("from-latest-survey")]
    public async Task<ActionResult<HealthPredictionResponseDto>> GetPredictionFromLatestSurvey()
    {
        var userId = GetCurrentUserId();
        
        var dailySurveys = await _dailySurveysService.GetDailySurveysForUser(userId);
        var latestSurvey = dailySurveys.OrderByDescending(s => s.CreationDateUtc).FirstOrDefault();
        
        if (latestSurvey == null)
        {
            return NotFound("No daily surveys found for the user");
        }
        
        var weeklySurveys = await _weeklySurveysService.GetWeeklySurveysForUser(userId);
        var currentWeek = 1;
        
        if (weeklySurveys.Any())
        {
            var latestWeekly = weeklySurveys.OrderByDescending(w => w.CreationDateUtc).FirstOrDefault();
            currentWeek = latestWeekly?.PregnancyWeek ?? 1;
        }
        
        var trimester = GetTrimester(currentWeek);
        var features = MapToModelFeatures(latestSurvey);
        
        var request = new HealthPredictionRequestDto
        {
            Features = features,
            Trimester = trimester,
            UserId = userId,
            DailySurveyId = latestSurvey.Id
        };
        
        var result = await _kafkaPredictionService.GetPredictionAsync(request);
        
        if (result == null)
        {
            return StatusCode(503, "Model service is unavailable");
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Получить прогноз здоровья на основе ежедневного обследования по ID
    /// </summary>
    [HttpGet("from-survey/{surveyId:guid}")]
    public async Task<ActionResult<HealthPredictionResponseDto>> GetPredictionFromSurvey(Guid surveyId)
    {
        var userId = GetCurrentUserId();
        
        var dailySurveys = await _dailySurveysService.GetDailySurveysForUser(userId);
        var survey = dailySurveys.FirstOrDefault(s => s.Id == surveyId);
        
        if (survey == null)
        {
            return NotFound("Survey not found");
        }
        
        var weeklySurveys = await _weeklySurveysService.GetWeeklySurveysForUser(userId);
        var surveyWeek = weeklySurveys
            .Where(w => w.CreationDateUtc <= survey.CreationDateUtc)
            .OrderByDescending(w => w.CreationDateUtc)
            .FirstOrDefault()?.PregnancyWeek ?? 1;
        
        var trimester = GetTrimester(surveyWeek);
        var features = MapToModelFeatures(survey);
        
        var request = new HealthPredictionRequestDto
        {
            Features = features,
            Trimester = trimester,
            UserId = userId,
            DailySurveyId = surveyId
        };
        
        var result = await _kafkaPredictionService.GetPredictionAsync(request);
        
        if (result == null)
        {
            return StatusCode(503, "Model service is unavailable");
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Отправить данные на прогноз напрямую
    /// </summary>
    [HttpPost("predict")]
    public async Task<ActionResult<HealthPredictionResponseDto>> Predict([FromBody] HealthPredictionRequestDto request)
    {
        if (request.Features == null || request.Features.Count != 21)
        {
            return BadRequest("Expected 21 features in the request");
        }
        
        if (request.Trimester < 1 || request.Trimester > 3)
        {
            return BadRequest("Trimester must be 1, 2, or 3");
        }
        
        var result = await _kafkaPredictionService.GetPredictionAsync(request);
        
        if (result == null)
        {
            return StatusCode(503, "Model service is unavailable");
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Проверить доступность сервиса модели
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    public async Task<ActionResult> CheckHealth()
    {
        var isAvailable = await _kafkaPredictionService.IsModelServiceAvailableAsync();
        
        if (isAvailable)
        {
            return Ok(new { status = "healthy", message = "Model service is available" });
        }
        
        return StatusCode(503, new { status = "unhealthy", message = "Model service is unavailable" });
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        
        return Guid.Parse(userIdClaim);
    }

    private int GetTrimester(int pregnancyWeek)
    {
        return pregnancyWeek switch
        {
            < 14 => 1,
            <= 26 => 2,
            _ => 3
        };
    }

    private List<double> MapToModelFeatures(DailySurveyDto survey)
    {
        var temperatureValue = survey.Temperature switch
        {
            Enums.DailySurvey.Temperature.Lower32And2 => 36.0m,
            Enums.DailySurvey.Temperature.Between37And2And37And5 => 37.3m,
            Enums.DailySurvey.Temperature.Higher37And5 => 38.0m,
            _ => 36.6m
        };
        
        return new List<double>
        {
            survey.AbdomenHurts ? 1 : 0,
            survey.Nausea ? 1 : 0,
            survey.UrgeToPuke,
            0,
            (double)temperatureValue,
            (double)(survey.Saturation ?? 98m),
            (double)(survey.Bld ?? 0),
            (double)(survey.Ket ?? 0),
            (double)(survey.Leu ?? 0),
            (double)(survey.Glu ?? 0),
            (double)(survey.Nit ?? 0),
            (double)(survey.Uro ?? 0),
            (double)(survey.Bil ?? 0),
            (double)(survey.Vc ?? 0),
            (double)(survey.Pro ?? 0),
            (double)(survey.Ph ?? 7m),
            (double)(survey.Sg ?? 1.015m),
            survey.SystolicPressure,
            survey.DiastolicPressure,
            survey.HeartRate,
            (double)(survey.GlucoseLevel ?? 5.0m)
        };
    }
}

