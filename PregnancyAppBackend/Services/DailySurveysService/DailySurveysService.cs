using Microsoft.EntityFrameworkCore;
using PregnancyAppBackend.Converters;
using PregnancyAppBackend.Dtos.Web.DailySurvey;
using PregnancyAppBackend.Dtos.Web;
using PregnancyAppBackend.Dtos.Web.DailyHealthPrediction;
using PregnancyAppBackend.Enums.DailySurvey;
using PregnancyAppBackend.Exceptions;
using PregnancyAppBackend.Persistance;
using PregnancyAppBackend.Services.HealthPredictionsService;
using PregnancyAppBackend.Services.KafkaPredictionService;
using PregnancyAppBackend.Services.UserInfoService;
using PregnancyAppBackend.Services.WeeklySurveyService;
using PregnancyAppBackend.Utils;

namespace PregnancyAppBackend.Services.DailySurveysService;

public class DailySurveysService : IDailySurveysService
{
    private readonly IDatabaseContext _databaseContext;
    private readonly IUserInfoService _userInfoService;
    private readonly ILogger<DailySurveysService> _logger;

    private readonly IKafkaPredictionService _kafkaPredictionService;
    private readonly IWeeklySurveysService _weeklySurveysService;
    private readonly IHealthPredictionsService _healthPredictionsService;

    public DailySurveysService(
        IDatabaseContext databaseContext,
        IUserInfoService userInfoService,
        ILogger<DailySurveysService> logger,
        IKafkaPredictionService kafkaPredictionService,
        IWeeklySurveysService weeklySurveysService,
        IHealthPredictionsService healthPredictionsService)
    {
        _databaseContext = databaseContext;
        _userInfoService = userInfoService;
        _logger = logger;
        _kafkaPredictionService = kafkaPredictionService;
        _weeklySurveysService = weeklySurveysService;
        _healthPredictionsService = healthPredictionsService;
    }

    public async Task<DailySurveyDto> AddDailySurvey(DailySurveyDto dailySurveyDto)
    {
        var userId = _userInfoService.GetUserInfoFromToken().UserId;

        _logger.LogInformation("Adding daily survey for userId={userId}, dto={@weeklySurveyDto}", userId, dailySurveyDto);

        var latestDateUtc = await GetLatestDailySurveyCreationDateUtcAsync();
        if (latestDateUtc.HasValue && !DateUtils.Has24HoursPassed(latestDateUtc.Value))
        {
            throw new ApiException($"Daily survey for user with id={userId} submitted less than 24h ago.",
                                   "Ежедневный опрос отправлен менее 24 часов назад. Попробуйте позже.");
        }

        var entity = await _databaseContext.DailySurveys.AddAsync(dailySurveyDto.ConvertToEntity());
        var dailySurveyEntity = entity.Entity;

        dailySurveyEntity.CreationDateUtc = DateTime.UtcNow;
        dailySurveyEntity.UserId = userId;

        await _databaseContext.SaveChangesAsync();

        var createdDto = dailySurveyEntity.ConvertToDto();

        // 1) Определяем триместр на основе последнего недельного опроса
        var weeklySurveys = await _weeklySurveysService.GetWeeklySurveysForUser(userId);
        var currentWeek = 1;

        if (weeklySurveys.Any())
        {
            var latestWeekly = weeklySurveys.OrderByDescending(w => w.CreationDateUtc).FirstOrDefault();
            currentWeek = latestWeekly?.PregnancyWeek ?? 1;
        }

        var trimester = GetTrimester(currentWeek);

        // 2) Маппим features для модели (21 признак)
        // ВАЖНО: делаем маппинг из входного DTO, чтобы не потерять temperatureRaw при конвертациях entity->dto.
        var features = MapToModelFeatures(dailySurveyDto);


        // 3) Отправляем запрос ML в Kafka (send & forget),
        //    без ожидания ответа и без синхронной зависимости от prediction-responses.
        var request = new HealthPredictionRequestDto
        {
            Features = features,
            Trimester = trimester,
            UserId = userId,
            DailySurveyId = createdDto.Id
        };

        await _kafkaPredictionService.SendPredictionRequestAsync(request);

        return createdDto;
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
        // Если пришло сырое дробное значение — используем его без какой-либо квантизации.
        // Иначе (старые клиенты) падаем на enum-классификацию.
        var temperatureValue = survey.TemperatureRaw.HasValue
            ? (decimal)survey.TemperatureRaw.Value
            : survey.Temperature switch
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

    public async Task<List<DailySurveyDto>> GetDailySurveysForUser(Guid userId, IDatabaseContext? databaseContext = null)
    {
        databaseContext ??= _databaseContext;
        
        var surveys = await databaseContext.DailySurveys
                                           .OrderByDescending(ds => ds.CreationDateUtc)
                                           .Where(ds => ds.UserId == userId)
                                           .ToListAsync();

        return surveys.Select(s => s.ConvertToDto()).ToList();
    }

    public async Task<DailySurveyDto> GetDailySurveyById(Guid surveyId)
    {
        var userId = _userInfoService.GetUserInfoFromToken().UserId;
        
        var survey = await _databaseContext.DailySurveys
                                           .SingleOrDefaultAsync(ds => ds.Id == surveyId);

        if (survey is null)
        {
            throw new ApiException($"Survey for userId={userId} with id={surveyId} not found.", 
                                   "Ошибка при получении ежедневного опроса. Попробуйте позже.");
        }

        if (!(survey.UserId == userId) && !await _userInfoService.CheckUserIsAdminAsync() && !await _userInfoService.CheckUserIsDoctorAsync())
        {
            throw new ApiException($"No access for survey for userId={userId} with id={surveyId}.", 
                                   "Ошибка при получении ежедневного опроса. Попробуйте позже.");
        }
        
        return survey.ConvertToDto();
    }

    public async Task<DateTime?> GetLatestDailySurveyCreationDateUtcAsync()
    {
        var userId = _userInfoService.GetUserInfoFromToken().UserId;

        return await _databaseContext.DailySurveys
                                     .OrderByDescending(ds => ds.CreationDateUtc)
                                     .Where(ds => ds.UserId == userId)
                                     .Select(ds => ds.CreationDateUtc)
                                     .FirstOrDefaultAsync();
    }
}