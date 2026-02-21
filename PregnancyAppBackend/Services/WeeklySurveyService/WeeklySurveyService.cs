using Microsoft.EntityFrameworkCore;
using PregnancyAppBackend.Converters;
using PregnancyAppBackend.Dtos.Web.WeeklySurvey;
using PregnancyAppBackend.Exceptions;
using PregnancyAppBackend.Persistance;
using PregnancyAppBackend.Services.UserInfoService;
using PregnancyAppBackend.Utils;

namespace PregnancyAppBackend.Services.WeeklySurveyService;

public class WeeklySurveysService : IWeeklySurveysService
{
    private readonly IDatabaseContext _databaseContext;
    private readonly IUserInfoService _userInfoService;
    private readonly ILogger<WeeklySurveysService> _logger;

    public WeeklySurveysService(IDatabaseContext databaseContext, 
                                IUserInfoService userInfoService, 
                                ILogger<WeeklySurveysService> logger)
    {
        _databaseContext = databaseContext;
        _userInfoService = userInfoService;
        _logger = logger;
    }

    public async Task<WeeklySurveyDto> AddWeeklySurvey(WeeklySurveyDto weeklySurveyDto)
    {
        var userId = _userInfoService.GetUserInfoFromToken().UserId;
        
        _logger.LogInformation("Adding weekly survey for userId={userId}, dto={@weeklySurveyDto}", userId, weeklySurveyDto);

        var latestDateUtc = await GetLatestWeeklySurveyDateAsync();
        if (latestDateUtc.HasValue && !DateUtils.HasWeekPassed(latestDateUtc.Value))
        {
            throw new ApiException($"Daily survey for user with id={userId} submitted less than a day ago", 
                                   "Ежедневный опрос уже был заполнен. Попробуйте позже");
        }
        
        var entity = await _databaseContext.WeeklySurveys.AddAsync(weeklySurveyDto.ConvertToEntity());
        var weeklySurveyEntity = entity.Entity;

        weeklySurveyEntity.CreationDateUtc = DateTime.UtcNow;
        weeklySurveyEntity.UserId = userId;

        await _databaseContext.SaveChangesAsync();

        return weeklySurveyEntity.ConvertToDto();
    }

    public async Task<List<WeeklySurveyDto>> GetWeeklySurveysForUser(Guid userId, IDatabaseContext? databaseContext = null)
    {
        databaseContext ??= _databaseContext;
        
        var surveys = await databaseContext.WeeklySurveys
                                           .OrderByDescending(ds => ds.CreationDateUtc)
                                           .Where(ds => ds.UserId == userId)
                                           .ToListAsync();

        return surveys.Select(s => s.ConvertToDto()).ToList();
    }

    public async Task<WeeklySurveyDto> GetWeeklySurveyById(Guid surveyId)
    {
        var userId = _userInfoService.GetUserInfoFromToken().UserId;
        
        var survey = await _databaseContext.WeeklySurveys
                                           .OrderByDescending(ds => ds.CreationDateUtc)
                                           .SingleOrDefaultAsync(ds => ds.Id == surveyId);

        if (survey is null)
        {
            throw new ApiException($"Survey for userId={userId} with id={surveyId} not found.", 
                                   "Ошибка при получении еженедельного опроса. Попробуйте позже.");
        }

        if (!(survey.UserId == userId) && !await _userInfoService.CheckUserIsAdminAsync() && !await _userInfoService.CheckUserIsDoctorAsync())
        {
            throw new ApiException($"No access for survey for userId={userId} with id={surveyId}.", 
                                   "Ошибка при получении еженедельного опроса. Попробуйте позже.");
        }
        
        return survey.ConvertToDto();
    }

    public async Task<DateTime?> GetLatestWeeklySurveyDateAsync()
    {
        var userId = _userInfoService.GetUserInfoFromToken().UserId;

        return await _databaseContext.WeeklySurveys
                                     .OrderByDescending(ds => ds.CreationDateUtc)
                                     .Where(ds => ds.UserId == userId)
                                     .Select(ds => ds.CreationDateUtc)
                                     .FirstOrDefaultAsync();
    }
}