using Microsoft.EntityFrameworkCore;
using PregnancyAppBackend.Converters;
using PregnancyAppBackend.Dtos.Web.DailySurvey;
using PregnancyAppBackend.Exceptions;
using PregnancyAppBackend.Persistance;
using PregnancyAppBackend.Services.UserInfoService;
using PregnancyAppBackend.Utils;

namespace PregnancyAppBackend.Services.DailySurveysService;

public class DailySurveysService : IDailySurveysService
{
    private readonly IDatabaseContext _databaseContext;
    private readonly IUserInfoService _userInfoService;
    private readonly ILogger<DailySurveysService> _logger;

    public DailySurveysService(IDatabaseContext databaseContext, IUserInfoService userInfoService, ILogger<DailySurveysService> logger)
    {
        _databaseContext = databaseContext;
        _userInfoService = userInfoService;
        _logger = logger;
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

        return dailySurveyEntity.ConvertToDto();
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