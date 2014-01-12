using Quartz;

namespace TeamMashup.Services
{
    public static class Jobs
    {
        public static void SpawnBillingJobs(this IScheduler scheduler)
        {
            if (!Configuration.Billing.IsEnabled)
                return;


            var billSubscriptionsJob = JobBuilder.Create<BillSubscriptionsJob>()
                                    .RequestRecovery(true)
                                    .WithDescription("Bill Subscription Job")
                                    .Build();

            var billSubscriptionsTrigger = TriggerBuilder.Create()
                                            .ForJob(billSubscriptionsJob)
                                            .WithDailyTimeIntervalSchedule(
                                                    x => x.WithIntervalInSeconds(Configuration.Billing.JobIntervalInSeconds).Build())
                                            .StartNow()
                                            .Build();

            scheduler.ScheduleJob(billSubscriptionsJob, billSubscriptionsTrigger);
        }

        public static void SpawnBackupJobs(this IScheduler scheduler)
        {
            if (!Configuration.Backup.IsEnabled)
                return;


            var backupJob = JobBuilder.Create<BackupJob>()
                                    .RequestRecovery(true)
                                    .WithDescription("Backup Job")
                                    .Build();

            var backupTrigger = TriggerBuilder.Create()
                                            .ForJob(backupJob)
                                            .WithDailyTimeIntervalSchedule(x => x.WithIntervalInSeconds(Configuration.Backup.JobIntervalInSeconds).Build())
                                            .StartNow()
                                            .Build();

            scheduler.ScheduleJob(backupJob, backupTrigger);
        }
    }
}
