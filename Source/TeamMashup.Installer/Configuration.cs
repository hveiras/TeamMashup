using System.Configuration;
using TeamMashup.Core;

namespace TeamMashup.Installer
{
    public static class Configuration
    {
        public static string ServerName
        {
            get
            {
                var value = ConfigurationManager.AppSettings.Get("ServerName");

                if (string.IsNullOrEmpty(value))
                    return @".\";

                return value;
            }
        }

        public static string BackupFileName
        {
            get 
            {
                var value = ConfigurationManager.AppSettings.Get("BackupFileName");

                if (string.IsNullOrEmpty(value))
                    return "Backup.bak";

                return value;
            }
        }

        public static string DatabaseName
        {
            get
            {
                var value = ConfigurationManager.AppSettings.Get("DatabaseName");

                if (string.IsNullOrEmpty(value))
                    return "TeamMashup";

                return value;
            }
        }

        public static string LoginName
        {
            get
            {
                var value = ConfigurationManager.AppSettings.Get("LoginName");

                if (string.IsNullOrEmpty(value))
                    return "teammashup";

                return value;
            }
        }

        public static string LoginPassword
        {
            get
            {
                var value = ConfigurationManager.AppSettings.Get("LoginPassword");

                if (string.IsNullOrEmpty(value))
                    return "test1234";

                return value;
            }
        }

        public static string SqlServiceName
        {
            get
            {
                var value = ConfigurationManager.AppSettings.Get("SqlServiceName");

                if (string.IsNullOrEmpty(value))
                    return "MSSQL$SQLEXPRESS";

                return value;
            }
        }

        public static string SiteName
        {
            get
            {
                var value = ConfigurationManager.AppSettings.Get("SiteName");

                if (string.IsNullOrEmpty(value))
                    return "teammashup";

                return value;
            }
        }

        public static string DeploymentPath
        {
            get
            {
                var value = ConfigurationManager.AppSettings.Get("DeploymentPath");

                if (string.IsNullOrEmpty(value))
                    return @"C:\Temp\Teammashup\Web";

                return value;
            }
        }

        public static int SitePort
        {
            get
            {
                return ConfigurationHelper.GetIntConfigurationProperty("SitePort", 8080);
            }
        }
    }
}
