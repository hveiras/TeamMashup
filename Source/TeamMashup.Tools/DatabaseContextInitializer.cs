using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using TeamMashup.Core;
using TeamMashup.Core.Domain;
using TeamMashup.Core.Enums;
using TeamMashup.Core.Security;
using TMFile = TeamMashup.Core.Domain.File;

namespace TeamMashup.Tests
{
    public class DatabaseContextInitializer : DropCreateDatabaseAlways<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            context.SystemSettings.Add(new SystemSetting("BackupIntervalInMinutes", "1440"));

            var roleAdministrator = new ProjectAssignmentRole("Administrator");
            var roleDeveloper = new ProjectAssignmentRole("Developer");
            var roleUser = new ProjectAssignmentRole("User");

            context.ProjectAssignmentRoles.Add(roleAdministrator);
            context.ProjectAssignmentRoles.Add(roleDeveloper);
            context.ProjectAssignmentRoles.Add(roleUser);

            var languageEnglish = new Language { Code = "en", Name = "English" };
            var languageSpanish = new Language { Code = "es", Name = "Español" };

            context.Languages.Add(languageEnglish);
            context.Languages.Add(languageSpanish);

            var teammashupAdmin = new User("Platform Admin", "admin@teammashup.com", SecurityManager.Hash("123"));
            var teammashupSales = new User("Sales Manager", "sales@teammashup.com", SecurityManager.Hash("123"));

            context.Users.Add(teammashupAdmin);
            context.Users.Add(teammashupSales);

            //var subscriptionPlanFree = new SubscriptionPlan("Free", 0, 10);
            //context.SubscriptionPlans.Add(subscriptionPlanFree);
            //context.SaveChanges();

            var subscriptionPlanProfessional = new SubscriptionPlan("Professional", 199, 20);
            context.SubscriptionPlans.Add(subscriptionPlanProfessional);
            context.SaveChanges();

            var subscriptionPlanEnterprise = new SubscriptionPlan("Enterprise", 399, 100);
            context.SubscriptionPlans.Add(subscriptionPlanEnterprise);
            context.SaveChanges();

            var subscriptionPlanUnlimited = new SubscriptionPlan("Unlimited", 599, int.MaxValue);
            context.SubscriptionPlans.Add(subscriptionPlanUnlimited);
            context.SaveChanges();

            var countryArgentina = new Country("Argentina");
            context.Countries.Add(countryArgentina);
            context.SaveChanges();

            var countryUsa = new Country("United States");
            context.Countries.Add(countryUsa);
            context.SaveChanges();

            string emailDomain;
            StringExtensions.TryParseEmailDomain("info@acme.com", out emailDomain);

            var subscriptionAcme = new Subscription("Acme", "Acme", "info@acme.com", emailDomain, subscriptionPlanProfessional.Id, countryArgentina.Id, "Av. Lacroze 1111");
            subscriptionAcme.State = SubscriptionState.Active;
            context.Subscriptions.Add(subscriptionAcme);
            context.SaveChanges();

            var acmeSebastian = new User("Sebastián Ramirez", "seba@contoso.com", SecurityManager.Hash("123"), subscriptionAcme.Id)
            {
                Enabled = true,
            };

            string path1 = Path.Combine(Environment.CurrentDirectory, "profile-small.png");
            byte[] content1 = System.IO.File.ReadAllBytes(path1);

            var file1 = new TMFile
            {
                SubscriptionId = subscriptionAcme.Id,
                UniqueId = Guid.NewGuid(),
                FileName = "profile-small",
                MimeType = "image/png",
                Content = content1
            };

            string path2 = Path.Combine(Environment.CurrentDirectory, "profile-big.png");
            byte[] content2 = System.IO.File.ReadAllBytes(path2);

            var file2 = new TMFile
            {
                SubscriptionId = subscriptionAcme.Id,
                UniqueId = Guid.NewGuid(),
                FileName = "profile-big",
                MimeType = "image/png",
                Content = content2
            };

            context.Files.Add(file1);
            context.Files.Add(file2);
            context.SaveChanges();

            var subscriptorAcmeProfile = new UserProfile
            {
                Language = languageEnglish
            };

            subscriptionAcme.SubscriptorId = acmeSebastian.Id;

            context.Users.Add(acmeSebastian);
            context.SaveChanges();

            subscriptorAcmeProfile.UserId = acmeSebastian.Id;
            context.UserProfiles.Add(subscriptorAcmeProfile);

            //Platform Claims
            var manageSecurityPlatformClaim = new Claim { Name = "manage-security", Scope = AssetScope.Platform };
            var readLogPlatformClaim = new Claim { Name = "read-log", Scope = AssetScope.Platform };
            var billingPlatformClaim = new Claim { Name = "manage-billing", Scope = AssetScope.Platform };

            context.Claims.Add(manageSecurityPlatformClaim);
            context.Claims.Add(readLogPlatformClaim);
            context.Claims.Add(billingPlatformClaim);

            //Tenant Claims
            var manageSubscriptionTenantClaim = new Claim { Name = "manage-subscription", Scope = AssetScope.Tenant };
            var manageUsersTenantClaim = new Claim { Name = "manage-users", Scope = AssetScope.Tenant };
            var manageProjectsTenantClaim = new Claim { Name = "manage-projects", Scope = AssetScope.Tenant };

            context.Claims.Add(manageSubscriptionTenantClaim);
            context.Claims.Add(manageUsersTenantClaim);
            context.Claims.Add(manageProjectsTenantClaim);

            var acmeCory = new User("Cory Jenkins", "cory.jenkins@acme.com", SecurityManager.Hash("123"), subscriptionAcme.Id);
            var acmeKim = new User("Kim Garon", "kim.garon@acme.com", SecurityManager.Hash("123"), subscriptionAcme.Id);
            var acmeMolly = new User("Molly Merritt", "molly.merritt@acme.com", SecurityManager.Hash("123"), subscriptionAcme.Id);
            var acmeJimmie = new User("Jimmie Mangan", "jimmie.mangan@acme.com", SecurityManager.Hash("123"), subscriptionAcme.Id);

            var acmeHernanProfile = new UserProfile
            {
                Language = languageEnglish
            };

            context.Users.Add(acmeCory);
            context.Users.Add(acmeKim);
            context.Users.Add(acmeMolly);
            context.Users.Add(acmeJimmie);
            context.SaveChanges();

            acmeHernanProfile.UserId = acmeCory.Id;
            context.UserProfiles.Add(acmeHernanProfile);

            //Create platform roles.
            var adminPlatformRole = new Role { Name = "Admin", Claims = new List<Claim> { manageSecurityPlatformClaim, readLogPlatformClaim }, Users = new List<User> { teammashupAdmin }, IsSystemRole = true, Scope = AssetScope.Platform };

            var salesPlatformRole = new Role
            {
                Name = "Sales",
                Claims = new List<Claim>
                {
                   billingPlatformClaim
                },
                Users = new List<User>
                {
                    teammashupSales
                },
                IsSystemRole = true,
                Scope = AssetScope.Platform
            };

            context.Roles.Add(adminPlatformRole);
            context.Roles.Add(salesPlatformRole);

            //Create tenant roles.
            var subscriptorTenantRole = new Role
            {
                Name = "Subscriptor",
                Claims = new List<Claim> { manageSubscriptionTenantClaim },
                IsSystemRole = true,
                Users = new List<User>
                {
                    acmeSebastian
                }
            };

            var adminTenantRole = new Role
            {
                Name = "Admin",
                Claims = new List<Claim> { manageProjectsTenantClaim, manageUsersTenantClaim },
                Users = new List<User> 
                { 
                    acmeKim,
                    acmeSebastian
                },
                IsSystemRole = true,
                Scope = AssetScope.Tenant
            };

            var readerTenantRole = new Role
            {
                Name = "Reader",
                IsSystemRole = true,
                Scope = AssetScope.Tenant
            };

            var contributorTenantRole = new Role
            {
                Name = "Contributor",
                IsSystemRole = true,
                Users = new List<User>
                {
                    acmeCory,
                    acmeMolly,
                    acmeJimmie
                },
                Scope = AssetScope.Tenant
            };

            context.Roles.Add(adminTenantRole);
            context.Roles.Add(readerTenantRole);
            context.Roles.Add(contributorTenantRole);
            context.Roles.Add(subscriptorTenantRole);

            var sampleProject = new Project(subscriptionAcme.Id, "SampleProject")
            {
                Description = "Powerful Metrics for High Performance Developers",
                Assignments = new List<ProjectAssignment>
                {
                    new ProjectAssignment { Active = true, UserId = acmeKim.Id, RoleId =  roleAdministrator.Id },
                    new ProjectAssignment { Active = true, UserId = acmeCory.Id, RoleId =  roleDeveloper.Id },
                    new ProjectAssignment { Active = true, UserId = acmeMolly.Id, RoleId =  roleDeveloper.Id },
                }
            };

            var anubisProject = new Project(subscriptionAcme.Id, "Anubis")
            {
                Description = "General purpose .NET library",
                Assignments = new List<ProjectAssignment>
                {
                    new ProjectAssignment { Active = true, UserId = acmeKim.Id, RoleId =  roleAdministrator.Id },
                    new ProjectAssignment { Active = true, UserId = acmeCory.Id, RoleId =  roleDeveloper.Id },
                    new ProjectAssignment { Active = true, UserId = acmeSebastian.Id, RoleId =  roleDeveloper.Id },
                }
            };

            var teammashupProject = new Project(subscriptionAcme.Id, "TeamMashup")
            {
                Description = "Minimallist Project Management",
                Assignments = new List<ProjectAssignment>
                {
                    new ProjectAssignment { Active = true, UserId = acmeKim.Id, RoleId =  roleAdministrator.Id },
                    new ProjectAssignment { Active = true, UserId = acmeCory.Id, RoleId =  roleDeveloper.Id },
                    new ProjectAssignment { Active = true, UserId = acmeMolly.Id, RoleId =  roleDeveloper.Id },
                }
            };

            context.Projects.Add(sampleProject);
            context.Projects.Add(anubisProject);
            context.Projects.Add(teammashupProject);

            context.SaveChanges();

            //Create backlog items.
            var cod001 = new Issue("Multiple requests causes login to fail with anti-forgery exception", IssueType.Defect, sampleProject.Id, acmeCory.Id)
            {
                Priority = 1
            };

            var cod002 = new Issue("Blocking server calls caused by requiring indexes to be non stale are preventing us to get data.", IssueType.Defect, sampleProject.Id, acmeMolly.Id)
            {
                Priority = 1
            };

            var cod003 = new Issue("Teams dashboard doesn't show appropriate dates (TimeZone issue)", IssueType.Defect, sampleProject.Id, acmeKim.Id)
            {
                Priority = 2
            };

            var cod004 = new Issue("Technology Facts chart should display 2 decimal numbers on hover.", IssueType.Defect, sampleProject.Id, acmeSebastian.Id)
            {
                Priority = 3
            };

            var cod005 = new Issue("Download actions in all the app should point to Visual Studio Gallery.", IssueType.Defect, sampleProject.Id, acmeCory.Id)
            {
                Priority = 1
            };

            var cod006 = new Issue("Eclipse Alpha Version", IssueType.UserStory, sampleProject.Id, acmeSebastian.Id)
            {
                Priority = 2,
                StoryPoints = 13
            };

            var cod007 = new Issue("Me as an user would like to let other services, like Coderbits, to use my facts info stored in SampleProject.", IssueType.UserStory, sampleProject.Id, acmeSebastian.Id)
            {
                Priority = 3,
                StoryPoints = 5
            };

            var cod008 = new Issue("Me as an user would like to know that Timeline supports Visual Studio achievements.", IssueType.UserStory, sampleProject.Id, acmeJimmie.Id)
            {
                Priority = 4,
                StoryPoints = 1
            };

            context.Issues.Add(cod001);
            context.Issues.Add(cod002);
            context.Issues.Add(cod003);
            context.Issues.Add(cod004);
            context.Issues.Add(cod005);
            context.Issues.Add(cod006);
            context.Issues.Add(cod007);
            context.Issues.Add(cod008);

            var tmp001 = new Issue("Implement wall in Internal and Server", IssueType.UserStory, teammashupProject.Id, acmeCory.Id)
            {
                Priority = 2,
                StoryPoints = 2
            };

            var tmp002 = new Issue("Frontend reports", IssueType.UserStory, teammashupProject.Id, acmeCory.Id)
            {
                Priority = 2,
                StoryPoints = 3
            };

            var tmp003 = new Issue("Surveys", IssueType.UserStory, teammashupProject.Id, acmeCory.Id)
            {
                Priority = 3,
                StoryPoints = 2
            };

            var tmp004 = new Issue("Implement chat in frontend, unify with existing chat in Backend", IssueType.UserStory, teammashupProject.Id, acmeCory.Id)
            {
                Priority = 2,
                StoryPoints = 3
            };

            var tmp005 = new Issue("Implement Search on site", IssueType.UserStory, teammashupProject.Id, acmeCory.Id)
            {
                Priority = 2,
                StoryPoints = 5
            };

            var tmp006 = new Issue("As a backend administrator, I want to moderate content posted", IssueType.UserStory, teammashupProject.Id, acmeCory.Id)
            {
                Priority = 4,
                StoryPoints = 3
            };

            var tmp007 = new Issue("Implement graphic reports on backend", IssueType.UserStory, teammashupProject.Id, acmeCory.Id)
            {
                Priority = 4,
                StoryPoints = 3
            };

            var tmp008 = new Issue("Implement log viewer on backend", IssueType.UserStory, teammashupProject.Id, acmeCory.Id)
            {
                Priority = 2,
                StoryPoints = 1
            };

            var tmp009 = new Issue("Implement Backup/Restore", IssueType.UserStory, teammashupProject.Id, acmeCory.Id)
            {
                Priority = 4,
                StoryPoints = 2
            };

            context.Issues.Add(tmp001);
            context.Issues.Add(tmp002);
            context.Issues.Add(tmp003);
            context.Issues.Add(tmp004);
            context.Issues.Add(tmp005);
            context.Issues.Add(tmp006);
            context.Issues.Add(tmp007);
            context.Issues.Add(tmp008);
            context.Issues.Add(tmp009);

            context.SaveChanges();
        }
    }
}