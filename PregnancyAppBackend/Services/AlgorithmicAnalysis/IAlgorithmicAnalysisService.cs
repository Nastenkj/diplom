using PregnancyAppBackend.Persistance;

namespace PregnancyAppBackend.Services.AlgorithmicAnalysis;

public interface IAlgorithmicAnalysisService
{
    Task UpdateDbResultFromSurveysAsync(Guid userId, IDatabaseContext? databaseContext = null);
    
    Task<List<Entities.AlgorithmicAnalysisParameterValue>> GetAlgorithmicAnalysisAsync(Guid userId);
}