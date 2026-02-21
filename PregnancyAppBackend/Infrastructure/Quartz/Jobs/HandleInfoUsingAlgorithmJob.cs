using Microsoft.EntityFrameworkCore;
using PregnancyAppBackend.Entities.Security;
using PregnancyAppBackend.Infrastructure.Quartz.Interfaces;
using PregnancyAppBackend.Persistance;
using PregnancyAppBackend.Services.AlgorithmicAnalysis;
using Quartz;

namespace PregnancyAppBackend.Infrastructure.Quartz.Jobs;

[DisallowConcurrentExecution]
public class HandleInfoUsingAlgorithmJob : IScheduledJob
{
    private readonly ILogger<HandleInfoUsingAlgorithmJob> _logger;
    private readonly IDbContextFactory<DatabaseContext> _contextFactory;
    private readonly IAlgorithmicAnalysisService _algorithmicAnalysisService;

    public HandleInfoUsingAlgorithmJob(IAlgorithmicAnalysisService algorithmicAnalysisService, 
                                       ILogger<HandleInfoUsingAlgorithmJob> logger, 
                                       IDbContextFactory<DatabaseContext> contextFactory)
    {
        _algorithmicAnalysisService = algorithmicAnalysisService;
        _logger = logger;
        _contextFactory = contextFactory;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await using var databaseContext = _contextFactory.CreateDbContext();

        var startTime = DateTimeOffset.Now;
        _logger.LogInformation("HandleInfoUsingAlgorithmJob started at: {time:yyyy-MM-dd HH:mm:ss.fff}", startTime);

        
        var patientGuids = await databaseContext.Users
                                                 .Where(u => u.Roles.Any(r => r.Id.ToString() == Role.PatientId))
                                                 .Select(u => u.Id)
                                                 .ToListAsync();

        var tasks = patientGuids.Select(pId => Handle(pId)).ToList();
        
        await Task.WhenAll(tasks);
        
        var endTime = DateTimeOffset.Now;
        var duration = endTime - startTime;
        _logger.LogInformation(
            "HandleInfoUsingAlgorithmJob completed at: {time:yyyy-MM-dd HH:mm:ss.fff}. Users handled: {tasksCount} Duration: {duration:mm\\:ss\\.fff}", 
            endTime, 
            tasks.Count(),
            duration
        );
    }

    private async Task Handle(Guid patientId)
    {
        await using var databaseContext = _contextFactory.CreateDbContext();

        await _algorithmicAnalysisService.UpdateDbResultFromSurveysAsync(patientId, databaseContext);

    }
}