namespace PregnancyAppBackend.Infrastructure.Quartz.Settings
{
    public class QuartzSettings
    {
        public List<JobSchedule> Jobs { get; set; } = new();
    }

    public class JobSchedule
    {
        /// <summary>
        /// Job name that corresponds to the class name without namespace
        /// For example: "ClearDeadLinksJob"
        /// </summary>
        public string JobName { get; set; } = string.Empty;
        
        /// <summary>
        /// Cron expression for job scheduling
        /// </summary>
        public string CronExpression { get; set; } = string.Empty;
        
        /// <summary>
        /// Optional data to pass to the job
        /// </summary>
        public Dictionary<string, string> JobData { get; set; } = new();
        
        /// <summary>
        /// Whether the job is enabled
        /// </summary>
        public bool IsEnabled { get; set; } = true;
    }
}