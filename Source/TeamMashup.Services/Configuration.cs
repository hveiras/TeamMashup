using System.Configuration;
using TeamMashup.Core;

namespace TeamMashup.Services
{
    public static class Configuration
    {
        public static class Billing
        {
            public static bool IsEnabled
            {
                get
                {
                    return ConfigurationHelper.GetBooleanConfigurationProperty("Billing.IsEnabled", true);
                }
            }

            public static int JobIntervalInSeconds
            {
                get
                {
                    return ConfigurationHelper.GetIntConfigurationProperty("Billing.JobIntervalInSeconds", 60);
                }
            }
        }

        public static class Backup
        {
            public static bool IsEnabled
            {
                get
                {
                    return ConfigurationHelper.GetBooleanConfigurationProperty("Backup.IsEnabled", true);
                }
            }

            public static string Directory
            {
                get
                {
                    var value = ConfigurationManager.AppSettings.Get("Backup.Directory");

                    if (string.IsNullOrEmpty(value))
                        return @"D:\Backups";

                    return value;
                }
            }

            public static string FileName
            {
                get
                {
                    var value = ConfigurationManager.AppSettings.Get("Backup.FileName");

                    if(string.IsNullOrEmpty(value))
                        return "TeamMashup";

                    return value;
                }
            }

            public static string ServerName
            {
                get
                {
                    var value = ConfigurationManager.AppSettings.Get("Backup.ServerName");

                    if (string.IsNullOrEmpty(value))
                        return @".\";

                    return value;
                }
            }

            public static int JobIntervalInSeconds
            {
                get
                {
                    return ConfigurationHelper.GetIntConfigurationProperty("Backup.JobIntervalInSeconds", 60);
                }
            }

            public static string DatabaseName
            {
                get
                {
                    var value = ConfigurationManager.AppSettings.Get("Backup.DatabaseName");

                    if (string.IsNullOrEmpty(value))
                        return "TeamMashup";

                    return value;
                }
            }
        }
    }
}
