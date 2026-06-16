using Microsoft.EntityFrameworkCore;
using PregnancyAppBackend.Dtos.Web;
using PregnancyAppBackend.Dtos.Web.DailyHealthPrediction;
using PregnancyAppBackend.Entities;
using PregnancyAppBackend.Persistance;
using PregnancyAppBackend.Services.UserInfoService;

namespace PregnancyAppBackend.Services.HealthPredictionsService;

public class HealthPredictionsService : IHealthPredictionsService
{
    private readonly IDatabaseContext _databaseContext;

    public HealthPredictionsService(IDatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task SavePredictionForDailySurveyAsync(
        Guid userId,
        Guid dailySurveyId,
        DateTime createdAtUtc,
        int trimester,
        int prediction,
        string predictionText,
        double normalProbability,
        double alertProbability,
        double pathologyProbability,
        IReadOnlyCollection<HealthPredictionDeviationDto> deviations,
        CancellationToken cancellationToken = default)
    {
        // На всякий случай: уникальность (UserId, DailySurveyId) задана в конфиге.
        // Поэтому делаем upsert.
        var existing = await _databaseContext.HealthPredictionResults
            .SingleOrDefaultAsync(x => x.UserId == userId && x.DailySurveyId == dailySurveyId, cancellationToken);

        if (existing is null)
        {
            existing = new HealthPredictionResult
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                DailySurveyId = dailySurveyId,
                CreatedAtUtc = createdAtUtc,
                Trimester = trimester,
                Prediction = prediction,
                PredictionText = predictionText,
                NormalProbability = normalProbability,
                AlertProbability = alertProbability,
                PathologyProbability = pathologyProbability,
                Deviations = deviations.Select(d => new HealthPredictionDeviation
                {
                    Feature = d.Feature,
                    Value = d.Value,
                    NormalRange = d.NormalRange,
                    Severity = d.Severity,
                    Message = d.Message,
                }).ToList()
            };

            await _databaseContext.HealthPredictionResults.AddAsync(existing, cancellationToken);
        }
        else
        {
            existing.CreatedAtUtc = createdAtUtc;
            existing.Trimester = trimester;
            existing.Prediction = prediction;
            existing.PredictionText = predictionText;
            existing.NormalProbability = normalProbability;
            existing.AlertProbability = alertProbability;
            existing.PathologyProbability = pathologyProbability;

            // Обновляем девиации: чистим и добавляем заново
            existing.Deviations.Clear();
            existing.Deviations = deviations.Select(d => new HealthPredictionDeviation
            {
                Feature = d.Feature,
                Value = d.Value,
                NormalRange = d.NormalRange,
                Severity = d.Severity,
                Message = d.Message,
            }).ToList();
        }

        await _databaseContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<DailyHealthPredictionResultDto>> GetDailyResultsForUserAsync(
        Guid userId,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var query = _databaseContext.HealthPredictionResults
            .Where(x => x.UserId == userId);

        if (fromUtc.HasValue)
        {
            query = query.Where(x => x.CreatedAtUtc >= fromUtc.Value);
        }

        if (toUtc.HasValue)
        {
            query = query.Where(x => x.CreatedAtUtc <= toUtc.Value);
        }

        var results = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new DailyHealthPredictionResultDto
            {
                DailySurveyId = x.DailySurveyId,
                CreationDateUtc = x.CreatedAtUtc,
                Trimester = x.Trimester,
                Prediction = x.Prediction,
                PredictionText = x.PredictionText,
                Probabilities = new PredictionProbabilitiesDto
                {
                    Normal = x.NormalProbability,
                    Alert = x.AlertProbability,
                    Pathology = x.PathologyProbability
                },
                Deviations = x.Deviations
                    .OrderBy(d => d.Id)
                    .Select(d => new DailyHealthPredictionDeviationDto
                    {
                        Feature = d.Feature,
                        Value = d.Value,
                        NormalRange = d.NormalRange,
                        Severity = d.Severity,
                        Message = d.Message
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return results;
    }

    public async Task<DailyHealthPredictionResultDto?> GetDailyResultByDailySurveyIdAsync(
        Guid userId,
        Guid dailySurveyId,
        CancellationToken cancellationToken = default)
    {
        // ML результаты привязаны к пациенту (UserId),
        // а endpoint получает dailySurveyId — поэтому берём пациента через DailySurvey.
        var dailySurvey = await _databaseContext.DailySurveys
            .SingleOrDefaultAsync(x => x.Id == dailySurveyId, cancellationToken);

        if (dailySurvey is null)
        {
            return null;
        }

        var patientId = dailySurvey.UserId;

        return await _databaseContext.HealthPredictionResults
            .Where(x => x.UserId == patientId && x.DailySurveyId == dailySurveyId)
            .Select(x => new DailyHealthPredictionResultDto
            {
                DailySurveyId = x.DailySurveyId,
                CreationDateUtc = x.CreatedAtUtc,
                Trimester = x.Trimester,
                Prediction = x.Prediction,
                PredictionText = x.PredictionText,
                Probabilities = new PredictionProbabilitiesDto
                {
                    Normal = x.NormalProbability,
                    Alert = x.AlertProbability,
                    Pathology = x.PathologyProbability
                },
                Deviations = x.Deviations
                    .OrderBy(d => d.Id)
                    .Select(d => new DailyHealthPredictionDeviationDto
                    {
                        Feature = d.Feature,
                        Value = d.Value,
                        NormalRange = d.NormalRange,
                        Severity = d.Severity,
                        Message = d.Message
                    })
                    .ToList()
            })
            .SingleOrDefaultAsync(cancellationToken);
    }
}
