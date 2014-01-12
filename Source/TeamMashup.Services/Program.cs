using System;
using System.Configuration.Install;
using System.Diagnostics;
using System.Reflection;
using TeamMashup.Core.Tracking;

namespace TeamMashup.Services
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting bootstrap service.");
                var service = new BootstrapService();

                if (Environment.UserInteractive)
                {
                    string parameter = string.Concat(args);
                    switch (parameter)
                    {
                        case "--install":
                            Console.WriteLine("Installing service.");
                            ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                            Console.WriteLine("Service installed.");
                            break;

                        case "--uninstall":
                            Console.WriteLine("Uninstalling service.");
                            ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                            Console.WriteLine("Service uninstalled.");
                            break;

                        default:
                            Console.WriteLine("Starting as User Interactive");
                            service.Start(false);
                            break;
                    }
                }
                else if (!Debugger.IsAttached)
                {
                    Console.WriteLine("Executing as windows service");
                    service.Start(true); // No debugger attached so we are free to execute as service
                }
                else
                {
                    Console.WriteLine("Executing on command-line. ");
                    service.Start(false); // Until async support will only stop when the debugger kills the service.
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex); 
            }
        }
    }
}