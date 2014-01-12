using Microsoft.SqlServer.Management.Smo;

namespace TeamMashup.Installer
{
    public static class ServerExtensions
    {
        public static bool DatabaseExists(this Server server, string databaseName)
        {
            return server.Databases[databaseName] != null;
        }

        public static void RestoreDatabase(this Server server, string databaseName, string filePath)
        {
            var res = new Restore();

            res.Devices.AddDevice(filePath, DeviceType.File);

            var DataFile = new RelocateFile();
            string MDF = res.ReadFileList(server).Rows[0][1].ToString();
            DataFile.LogicalFileName = res.ReadFileList(server).Rows[0][0].ToString();
            DataFile.PhysicalFileName = server.Databases[databaseName].FileGroups[0].Files[0].FileName;

            var LogFile = new RelocateFile();
            string LDF = res.ReadFileList(server).Rows[1][1].ToString();
            LogFile.LogicalFileName = res.ReadFileList(server).Rows[1][0].ToString();
            LogFile.PhysicalFileName = server.Databases[databaseName].LogFiles[0].FileName;

            res.RelocateFiles.Add(DataFile);
            res.RelocateFiles.Add(LogFile);

            res.Database = databaseName;
            res.NoRecovery = false;
            res.ReplaceDatabase = true;
            res.SqlRestore(server);
        }

        public static Database CreateDatabase(this Server server, string databaseName)
        {
            var db = new Database(server, databaseName);
            db.Create();

            return db;
        }

        public static Login CreateSqlLogin(this Server server, string loginName, string password, string defaultDatabase)
        {
            var login = new Login(server, loginName);
            login.DefaultDatabase = defaultDatabase;
            login.LoginType = LoginType.SqlLogin;
            login.PasswordPolicyEnforced = false;
            login.Create(password);
            login.Enable();

            return login;
        }

        public static bool LoginExists(this Server server, string loginName)
        {
            return server.Logins[loginName] != null;
        }
    }
}
