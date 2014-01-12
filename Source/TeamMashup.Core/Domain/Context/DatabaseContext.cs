using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace TeamMashup.Core.Domain
{
    public class DatabaseContext : DbContext, IDatabaseContext
    {
        public DbSet<Role> Roles { get; set; }

        public DbSet<Claim> Claims { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<File> Files { get; set; }

        public DbSet<Language> Languages { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<PasswordRecovery> PasswordRecoveries { get; set; }

        public DbSet<UserActivation> UserActivations { get; set; }

        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

        public DbSet<SubscriptionPlanHistory> SubscriptionPlanHistories { get; set; }

        public DbSet<Issue> Issues { get; set; }

        public DbSet<IssueProgress> IssueProgresses { get; set; }

        public DbSet<Release> Releases { get; set; }

        public DbSet<Iteration> Iterations { get; set; }

        public DbSet<Bill> Bills { get; set; }

        public DbSet<Log> Logs { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<ProjectAssignmentRole> ProjectAssignmentRoles { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<SystemSetting> SystemSettings { get; set; }

        public DbSet<BackupRequest> BackupRequests { get; set; }

        public DbSet<Survey> Surveys { get; set; }

        public DbSet<SurveyItem> SurveyItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Configurations.Add(new BillConfiguration());
            modelBuilder.Configurations.Add(new BillItemConfiguration());
            modelBuilder.Configurations.Add(new ClaimConfiguration());
            modelBuilder.Configurations.Add(new CountryConfiguration());
            modelBuilder.Configurations.Add(new FileConfiguration());
            modelBuilder.Configurations.Add(new RoleConfiguration());
            modelBuilder.Configurations.Add(new IterationConfiguration());
            modelBuilder.Configurations.Add(new IterationResourceConfiguration());
            modelBuilder.Configurations.Add(new LanguageConfiguration());
            modelBuilder.Configurations.Add(new LogConfiguration());
            modelBuilder.Configurations.Add(new PasswordRecoveryConfiguration());
            modelBuilder.Configurations.Add(new ProjectConfiguration());
            modelBuilder.Configurations.Add(new ProjectAssignmentConfiguration());
            modelBuilder.Configurations.Add(new ReleaseConfiguration());
            modelBuilder.Configurations.Add(new SubscriptionConfiguration());
            modelBuilder.Configurations.Add(new SubscriptionPlanConfiguration());
            modelBuilder.Configurations.Add(new SubscriptionPlanHistoryConfiguration());
            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new UserActivationConfiguration());
            modelBuilder.Configurations.Add(new UserProfileConfiguration());
            modelBuilder.Configurations.Add(new IssueConfiguration());
            modelBuilder.Configurations.Add(new IssueProgressConfiguration());
            modelBuilder.Configurations.Add(new CommentConfiguration());
            modelBuilder.Configurations.Add(new SystemSettingConfiguration());
            modelBuilder.Configurations.Add(new BackupRequestConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}