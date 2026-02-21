using PregnancyAppBackend.Dtos.Web.WeeklySurvey;
using PregnancyAppBackend.Persistance;

namespace PregnancyAppBackend.Services.WeeklySurveyService;

public interface IWeeklySurveysService
{
    Task<WeeklySurveyDto> AddWeeklySurvey(WeeklySurveyDto weeklySurveyDto);
    Task<List<WeeklySurveyDto>> GetWeeklySurveysForUser(Guid userId, IDatabaseContext? databaseContext = null);
    Task<WeeklySurveyDto> GetWeeklySurveyById(Guid surveyId);
    Task<DateTime?> GetLatestWeeklySurveyDateAsync();
}