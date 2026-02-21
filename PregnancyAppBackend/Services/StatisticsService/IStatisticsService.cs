
using PregnancyAppBackend.Dtos.Web.Statistics;
using PregnancyAppBackend.Dtos.Web.Statistics.Date;
using PregnancyAppBackend.Persistance;

namespace PregnancyAppBackend.Services.StatisticsService;

public interface IStatisticsService
{
    Task<StatisticsDatePlotResultDto> GetStatisticByDateAsync(string dbFieldName, DateTime? startDateUtc, DateTime? endDateUtc, Guid patientId, IDatabaseContext? databaseContext = null);
    Task<List<ObservationParameterWithNormDto>> GetObservationParametersStatistics(Guid userId);
}