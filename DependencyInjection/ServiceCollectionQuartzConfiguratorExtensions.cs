using Quartz;

namespace SimarAlertNotifier.DependencyInjection
{
    public static class ServiceCollectionQuartzConfiguratorExtensions
    {
        public static void AddQuartzScheduler<T>(
            this IServiceCollectionQuartzConfigurator services)
            where T : IJob
        {
            string jobName = typeof(T).Name;

            var configKey = $"Scheduler_{jobName}";
            var cronExpression = Environment.GetEnvironmentVariable(configKey);

            if (string.IsNullOrEmpty(cronExpression))
            {
                throw new Exception($"Cron expression for job {jobName} is not found in configuration.");
            }

            var jobKey = new JobKey(jobName);
            services.AddJob<T>(jobKey);
            services.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity(jobName + "-Trigger")
                .WithCronSchedule(cronExpression));
        }
    }
}
