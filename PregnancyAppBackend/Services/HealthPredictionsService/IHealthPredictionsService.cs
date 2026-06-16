using PregnancyAppBackend.Dtos.Web.DailyHealthPrediction;

namespace PregnancyAppBackend.Services.HealthPredictionsService;

public interface IHealthPredictionsService
{
    Task SavePredictionForDailySurveyAsync(
        Guid userId,
        Guid dailySurveyId,
        DateTime createdAtUtc,
        int trimester,
        int prediction,
        string predictionText,
        double normalProbability,
        double alertProbability,
        double pathologyProbability,
        IReadOnlyCollection<PregnancyAppBackend.Dtos.Web.HealthPredictionDeviationDto> deviations,
        CancellationToken cancellationToken = default);

    Task<List<DailyHealthPredictionResultDto>> GetDailyResultsForUserAsync(
        Guid userId,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default);

    Task<DailyHealthPredictionResultDto?> GetDailyResultByDailySurveyIdAsync(
        Guid userId,
        Guid dailySurveyId,
        CancellationToken cancellationToken = default);
}
