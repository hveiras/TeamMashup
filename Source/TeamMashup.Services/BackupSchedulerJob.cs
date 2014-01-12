using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamMashup.Core;
using TeamMashup.Core.Domain;
using TeamMashup.Core.Tracking;

namespace TeamMashup.Services
{
    [DisallowConcurrentExecution]
    public class BackupSchedulerJob
    {
        public void Execute(IJobExecutionContext jobContext)
        {
            try
            {
                using (var context = new DatabaseContext())
                {
                    int defaultInterval = 1440; //One day in minutes.
                    int interval;

                    SystemSetting intervalSettings;
                    if (!context.SystemSettings.TryGet("BackupIntervalInMinutes", out intervalSettings))
                        interval = defaultInterval;

                    if (!int.TryParse(intervalSettings.Value, out interval))
                        interval = defaultInterval;


                    BackupRequest request;
                    if (context.BackupRequests.TryGetNext(BackupSchedule.Automatic, out request))
                    {

                    }
                    else
                    {
                        //This is the first automatic backup.
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                ConsoleHelper.WriteLine(ConsoleColor.Magenta, ex.Message);
            }
        }
    }
}
