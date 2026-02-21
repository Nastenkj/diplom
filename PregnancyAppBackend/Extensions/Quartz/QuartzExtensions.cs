using PregnancyAppBackend.Infrastructure.Quartz.Interfaces;
using PregnancyAppBackend.Infrastructure.Quartz.Settings;
using Quartz;
using System.Reflection;

namespace PregnancyAppBackend.Extensions.Quartz;

    public static class QuartzExtensions
    {
        public static IServiceCollection AddQuartzServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind Quartz settings from appsettings
            var quartzSettings = new QuartzSettings();
            configuration.GetSection("Quartz").Bind(quartzSettings);

            // Find all job types in the assembly
            var jobTypes = FindAllJobTypes();
            
            // Register all jobs with DI
            RegisterJobsWithDI(services, jobTypes);
            
            // Add Quartz services
            services.AddQuartz(q => {
                // Add jobs from settings
                ConfigureJobsFromSettings(q, quartzSettings, jobTypes);
            });
            
            // Add the Quartz hosted service
            services.AddQuartzHostedService(options => {
                options.WaitForJobsToComplete = true;
                options.StartDelay = TimeSpan.FromSeconds(5);
            });
            
            return services;
        }
        
        private static List<Type> FindAllJobTypes()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IScheduledJob).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToList();
        }
        
        private static void RegisterJobsWithDI(IServiceCollection services, List<Type> jobTypes)
        {
            foreach (var jobType in jobTypes)
            {
                services.AddTransient(jobType);
            }
        }
        
        private static void ConfigureJobsFromSettings(
            IServiceCollectionQuartzConfigurator quartz, 
            QuartzSettings settings,
            List<Type> allJobTypes)
        {
            if (settings?.Jobs == null || !settings.Jobs.Any())
            {
                return;
            }
            
            // Create a dictionary for quick job type lookup by name
            var jobTypeMap = allJobTypes.ToDictionary(
                t => t.Name,
                t => t
            );
            
            foreach (var jobConfig in settings.Jobs.Where(j => j.IsEnabled))
            {
                // Find job type by name
                if (!jobTypeMap.TryGetValue(jobConfig.JobName, out var jobType))
                {
                    throw new Exception($"Job '{jobConfig.JobName}' not found. Make sure a class with this exact name exists and implements IScheduledJob.");
                }
                
                // Create job with simple name-based identity
                var jobKey = new JobKey(jobConfig.JobName);
                
                // Add the job
                quartz.AddJob(jobType, jobKey, opts => {
                    // Add job data if specified
                    if (jobConfig.JobData.Any())
                    {
                        // Create a new JobDataMap and add all entries
                        var jobDataMap = new JobDataMap();
                        foreach (var data in jobConfig.JobData)
                        {
                            jobDataMap.Put(data.Key, data.Value);
                        }
                        opts.SetJobData(jobDataMap);
                    }
                });
                
                // Add the trigger
                quartz.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity($"{jobConfig.JobName}-trigger")
                    .WithCronSchedule(jobConfig.CronExpression));
            }
        }
    }