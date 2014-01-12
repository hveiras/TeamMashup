using System;
using System.Data.Entity;

namespace TeamMashup.Core.Domain
{
    public interface IDatabaseContext : IDisposable
    {
        DbSet<Role> Roles { get; set; }

        DbSet<Claim> Claims { get; set; }

        DbSet<User> Users { get; set; }

        DbSet<UserProfile> UserProfiles { get; set; }

        DbSet<Subscription> Subscriptions { get; set; }

        DbSet<File> Files { get; set; }

        DbSet<Language> Languages { get; set; }

        DbSet<Project> Projects { get; set; }

        DbSet<PasswordRecovery> PasswordRecoveries { get; set; }

        DbSet<UserActivation> UserActivations { get; set; }

        DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

        DbSet<SubscriptionPlanHistory> SubscriptionPlanHistories { get; set; }

        DbSet<Issue> Issues { get; set; }

        DbSet<IssueProgress> IssueProgresses { get; set; }

        DbSet<Release> Releases { get; set; }

        DbSet<Iteration> Iterations { get; set; }

        DbSet<Bill> Bills { get; set; }

        DbSet<Log> Logs { get; set; }

        DbSet<Country> Countries { get; set; }

        DbSet<ProjectAssignmentRole> ProjectAssignmentRoles { get; set; }

        DbSet<Comment> Comments { get; set; }

        DbSet<SystemSetting> SystemSettings { get; set; }

        DbSet<BackupRequest> BackupRequests { get; set; }

        DbSet<Survey> Surveys { get; set; }

        DbSet<SurveyItem> SurveyItems { get; set; }

        int SaveChanges();
    }
}