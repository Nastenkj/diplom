using PregnancyAppBackend.Dtos.Web.DailySurvey;
using PregnancyAppBackend.Persistance;

namespace PregnancyAppBackend.Services.DailySurveysService;

public interface IDailySurveysService
{
    Task<DailySurveyDto> AddDailySurvey(DailySurveyDto dailySurveyDto);
    Task<List<DailySurveyDto>> GetDailySurveysForUser(Guid userId, IDatabaseContext? databaseContext = null);    
    Task<DailySurveyDto> GetDailySurveyById(Guid surveyId);
    Task<DateTime?> GetLatestDailySurveyCreationDateUtcAsync();
}