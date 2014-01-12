using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Wmi;
using Microsoft.Web.Administration;
using Microsoft.Win32;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using TeamMashup.Core.Domain;
using TeamMashup.Tests;
using TeamMashup.Tools.Helpers;
using File = System.IO.File;
using User = Microsoft.SqlServer.Management.Smo.User;

namespace TeamMashup.Installer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerConnection connection = null;

            try
            {
                connection = new ServerConnection();
                connection.ServerInstance = Configuration.ServerName;
                var server = new Server(connection);

                EnableMixedAuthenticationMode(server);

                var database = CreateDatabaseIfNotExists(server);

                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var backupFilePath = Path.Combine(path, Configuration.BackupFileName);

                try
                {
                    Console.WriteLine("Restoring database from backup {0}", backupFilePath);
                    server.RestoreDatabase(Configuration.DatabaseName, backupFilePath);
                }
                catch
                {
                    Console.WriteLine("Backup Restore Failed. Creating database from tests");
                    System.Data.Entity.Database.SetInitializer(new DatabaseContextInitializer());

                    using (var context = new DatabaseContext())
                    {
                        context.Database.Initialize(true);
                    }
                }

                CreateLoginIfNotExists(server, database);

                DeployPackage();

                CreateWebSiteIfNotExists();

                PostInstallConfiguration();
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLine(ConsoleColor.Magenta, string.Format("{0}\n{1}", ex.Message, ex.StackTrace));
            }
            finally
            {
                if(connection != null)
                    connection.Disconnect();

                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
        }

        private static Database CreateDatabaseIfNotExists(Server server)
        {
            Database database = null;
            if (server.DatabaseExists(Configuration.DatabaseName))
            {
                Console.WriteLine("Database {0} already exists", Configuration.DatabaseName);
                database = server.Databases[Configuration.DatabaseName];
            }
            else
            {
                Console.WriteLine("Database {0} does not exist, creating...", Configuration.DatabaseName);
                database = server.CreateDatabase(Configuration.DatabaseName);
            }

            return database;
        }

        private static void CreateLoginIfNotExists(Server server, Database database)
        {
            Login login = null;
            if (!server.LoginExists(Configuration.LoginName))
            {
                Console.WriteLine("Creating Login {0}", Configuration.LoginName);
                login = server.CreateSqlLogin(Configuration.LoginName, Configuration.LoginPassword, Configuration.DatabaseName);
            }
            else
            {
                login = server.Logins[Configuration.LoginName];
            }

            if (!database.UserExists(Configuration.LoginName))
            {
                Console.WriteLine("Creating User {0} for Database {1}", login.Name, database.Name);
                var user = new User(database, Configuration.LoginName);
                user.Login = login.Name;
                user.Create();
                user.AddToRole("db_owner");
            }
            else
            {
                ConsoleHelper.WriteLine(ConsoleColor.Yellow, string.Format("User {0} already exists in database {1}, creation skiped", Configuration.LoginName, Configuration.DatabaseName));
            }
            
        }

        private static void DeployPackage()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            const string packageName = "Package.zip";
            var sourceFile = Path.Combine(path, packageName);
            var destinationFile = Path.Combine(Configuration.DeploymentPath, packageName);

            if (!Directory.Exists(Configuration.DeploymentPath))
            {
                Console.WriteLine("Creating directory {0}", Configuration.DeploymentPath);
                Directory.CreateDirectory(Configuration.DeploymentPath);
            }

            Console.WriteLine("Copying file {0} to {1}", sourceFile, destinationFile);
            File.Copy(sourceFile, destinationFile, true);

            Console.WriteLine("Extracting file {0}", destinationFile);
            ZipFile.ExtractToDirectory(destinationFile, Configuration.DeploymentPath);

            File.Delete(destinationFile);
        }

        private static void CreateWebSiteIfNotExists()
        {
            var manager = new ServerManager();

            if (!manager.ApplicationPoolExists(Configuration.SiteName))
            {
                var pool = manager.ApplicationPools.Add(Configuration.SiteName);
                pool.ManagedRuntimeVersion = "v4.0";
                manager.CommitChanges();
            }
            else
            {
                ConsoleHelper.WriteLine(ConsoleColor.Yellow, string.Format("ApplicationPool {0} already exists, creation skiped", Configuration.SiteName));
            }

            if (!manager.SiteExists(Configuration.SiteName))
            {
                var site = manager.Sites.Add(Configuration.SiteName, Configuration.DeploymentPath, Configuration.SitePort);
                site.ApplicationDefaults.ApplicationPoolName = Configuration.SiteName;
                manager.CommitChanges();
            }
            else
            {
                ConsoleHelper.WriteLine(ConsoleColor.Yellow, string.Format("Site {0} already exists, creation skiped", Configuration.SiteName));
            }
        }

        private static void EnableMixedAuthenticationMode(Server server)
        {
            Console.WriteLine("Enabling Mixed Authentication Mode");

            var mc = new ManagedComputer();

            var is64 = Environment.Is64BitOperatingSystem;

            if (is64)
            {
                mc.ConnectionSettings.ProviderArchitecture = ProviderArchitecture.Use64bit;
            }

            var service = mc.Services[Configuration.SqlServiceName];

            service.StartAndWait();

            server.Settings.LoginMode = ServerLoginMode.Mixed;
            server.Settings.Alter();

            service.StopAndWait();

            service.EnableTCP(mc);

            service.StartAndWait();
        }

        private static void PostInstallConfiguration()
        {
            Console.WriteLine("Enabling Fonts download");
            
            var keyName = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\3";
            var key = Registry.CurrentUser.OpenSubKey(keyName, true);

            if (key != null)
            {
                key.SetValue("1604", 0, RegistryValueKind.DWord);
                key.Close();
            }
            else
            {
                ConsoleHelper.WriteLine(ConsoleColor.Magenta, string.Format("Key '{0}' not found", keyName));
            }
        }
    }
}