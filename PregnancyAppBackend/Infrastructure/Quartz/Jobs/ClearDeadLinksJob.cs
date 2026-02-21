using PregnancyAppBackend.Infrastructure.Quartz.Interfaces;
using PregnancyAppBackend.Services.CommunicationLinkService;
using Quartz;

namespace PregnancyAppBackend.Infrastructure.Quartz.Jobs;

[DisallowConcurrentExecution]
public class ClearDeadLinksJob : IScheduledJob
{
    private readonly ILogger<ClearDeadLinksJob> _logger;
    private readonly ICommunicationLinkService _communicationLinkService;

    public ClearDeadLinksJob(ILogger<ClearDeadLinksJob> logger, 
                             ICommunicationLinkService communicationLinkService)
    {
        _logger = logger;
        _communicationLinkService = communicationLinkService;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("ClearDeadLinksJob started at: {time}", DateTimeOffset.Now);

        await _communicationLinkService.ClearDeadCommunicationLinksAsync(TimeSpan.FromMinutes(1));

        _logger.LogInformation("ClearDeadLinksJob completed at: {time}", DateTimeOffset.Now);
    }
}