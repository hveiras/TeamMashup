using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using TeamMashup.Core.Domain;

namespace TeamMashup.Tests
{
    [TestClass]
    public class DatabaseTest
    {
        [TestMethod]
        [DeploymentItem(@"Content\")]
        public void CreateDatabase()
        {
            Database.SetInitializer(new DatabaseContextInitializer());

            //This is to force other active connections to close so the database can be droped/created
            SetDatabaseAsSingleUser();

            using (var context = new DatabaseContext())
            {
                context.Database.Initialize(true);
            }

            SetDatabaseAsMultiUser();
        }

        private void SetDatabaseAsSingleUser()
        {
            using (var context = new DatabaseContext())
            {
                context.Database.ExecuteSqlCommand("ALTER DATABASE TeamMashup SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
            }
        }

        private void SetDatabaseAsMultiUser()
        {
            using (var context = new DatabaseContext())
            {
                context.Database.ExecuteSqlCommand("ALTER DATABASE TeamMashup SET MULTI_USER");
            }
        }
    }
}