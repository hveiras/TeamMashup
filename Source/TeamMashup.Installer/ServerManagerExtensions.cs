using Microsoft.Web.Administration;

namespace TeamMashup.Installer
{
    public static class ServerManagerExtensions
    {
        public static bool ApplicationPoolExists(this ServerManager manager, string name)
        {
            return manager.ApplicationPools[name] != null;
        }

        public static bool SiteExists(this ServerManager manager, string name)
        {
            return manager.Sites[name] != null;
        }
    }
}
