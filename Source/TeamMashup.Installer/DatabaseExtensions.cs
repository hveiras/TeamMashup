using Microsoft.SqlServer.Management.Smo;

namespace TeamMashup.Installer
{
    public static class DatabaseExtensions
    {
        public static bool UserExists(this Database database, string userName)
        {
            return database.Users[userName] != null;
        }
    }
}
