using Microsoft.SqlServer.Management.Smo;
using Quartz;
using System;
using System.IO;
using TeamMashup.Core.Domain;
using TeamMashup.Core.Enums;
using TeamMashup.Core.Tracking;
using TeamMashup.Tools.Helpers;

namespace TeamMashup.Services
{
    [DisallowConcurrentExecution]
    public class BackupJob : IJob
    {
        public void Execute(IJobExecutionContext jobContext)
        {
            try
            {
                var backupDirectory = Configuration.Backup.Directory.EndsWith(@"\") ?
                        Configuration.Backup.Directory.TrimEnd('\\') :
                        Configuration.Backup.Directory;

                if (!Directory.Exists(backupDirectory))
                    Directory.CreateDirectory(backupDirectory);

                BackupRequest request;
                using (var context = new DatabaseContext())
                {
                    context.BackupRequests.TryGetNext(out request);
                }

                if (request != null)
                {
                    var backupName = string.Format(@"{0}\{1}-{2}-{3}.bak", backupDirectory, Configuration.Backup.FileName, request.Schedule.ToString(), DateTime.UtcNow.ToString("yyyy-MM-dd-H-mm"));

                    ConsoleHelper.WriteLine(ConsoleColor.Green, string.Format("Backup Job: Starting backup '{0}'", backupName));

                    var backup = new Backup();
                    backup.Action = BackupActionType.Database;
                    backup.Database = Configuration.Backup.DatabaseName;
                    backup.Devices.AddDevice(backupName, DeviceType.File);
                    backup.BackupSetName = "TeamMashup Backup";
                    backup.BackupSetDescription = "TeamMashup Backup";
                    backup.ExpirationDate = DateTime.UtcNow.AddMonths(1);

                    var server = new Server(Configuration.Backup.ServerName);
                    backup.SqlBackup(server);

                    using (var context = new DatabaseContext())
                    {
                        var storedRequest = context.BackupRequests.GetById(request.Id);
                        storedRequest.State = BackupState.Completed;
                        storedRequest.Path = backupName;
                        context.SaveChanges();
                    }

                    Console.WriteLine("Backup Job: Successfuly finished backup '{0}'", backupName);
                }
                else
                {
                    Console.WriteLine("Backup Job: No backup requests found.");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                ConsoleHelper.WriteLine(ConsoleColor.Magenta, "Backup Job: " + ex.Message);
            }
        }
    }
}
