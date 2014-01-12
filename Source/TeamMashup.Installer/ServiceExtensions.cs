using Microsoft.SqlServer.Management.Smo.Wmi;
using System;
using System.Threading;
using TeamMashup.Tools.Helpers;

namespace TeamMashup.Installer
{
    public static class ServiceExtensions
    {
        public static void StartAndWait(this Service service)
        {
            if (service.ServiceState != ServiceState.Running)
            {
                Console.WriteLine("Starting service {0}", service.Name);
                service.Start();
                service.Alter();

                while (service.ServiceState != ServiceState.Running)
                {
                    service.Refresh();
                    Thread.Sleep(100);
                }
            }
        }

        public static void StopAndWait(this Service service)
        {
            if (service.ServiceState == ServiceState.Running)
            {
                Console.WriteLine("Stopping service {0}", service.Name);
                service.Stop();
                service.Alter();

                while (service.ServiceState != ServiceState.Stopped)
                {
                    service.Refresh();
                    Thread.Sleep(100);
                }
            }
        }

        public static void EnableTCP(this Service service, ManagedComputer mc)
        {
            var serverProtocol = mc.ServerInstances[0].ServerProtocols["Tcp"];

            if (!serverProtocol.IsEnabled)
            {
                Console.WriteLine("Enabling TCP connections");

                serverProtocol.IsEnabled = true;
                serverProtocol.Alter();
            }
            else
            {
                ConsoleHelper.WriteLine(ConsoleColor.Yellow, "TCP connections already enabled");
            }
        }
    }
}
