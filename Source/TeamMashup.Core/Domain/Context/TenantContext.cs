using System;
using System.Collections.Generic;
using System.Linq;
using TeamMashup.Core.Domain.Services;

namespace TeamMashup.Core.Domain
{
    public class TenantContext : IDisposable, IContext
    {
        public IList<Tuple<string, string>> Errors { get; private set; }

        public IDatabaseContext Database { get; private set; }

        public long SubscriptionId { get; private set; }

        /// <summary>
        /// Use this context to handle all related with a Tenant (also called Subscription)
        /// </summary>
        public TenantContext(long subscriptionId, IDatabaseContext context = null)
        {
            SubscriptionId = subscriptionId;
            Database = context ?? new DatabaseContext();
            Errors = new List<Tuple<string, string>>();
        }

        /// <summary>
        /// Use this context to handle all operations related with a single project.
        /// </summary>
        /// <param name="projectId">The id of the Project</param>
        /// <returns></returns>
        public ProjectContext Project(long projectId)
        {
            return new ProjectContext(Database, projectId);
        }

        public IQueryable<User> Users
        {
            get
            {
                return Database.Users.FilterBySubscription(SubscriptionId);
            }
        }

        public IQueryable<Project> Projects
        {
            get
            {
                return Database.Projects.FilterBySubscription(SubscriptionId);
            }
        }

        public IQueryable<Role> Roles
        {
            get
            {
                return Database.Roles.TenantRoles();
            }
        }

        public IQueryable<File> Files
        {
            get
            {
                return Database.Files.FilterBySubscription(SubscriptionId);
            }
        }

        public IQueryable<Comment> Comments
        {
            get
            {
                return Database.Comments.FilterBySubscription(SubscriptionId);
            }
        }

        private LogService _logs;

        public LogService Logs
        {
            get
            {
                if (_logs == null)
                    _logs = new LogService(SubscriptionId, Database);

                return _logs;
            }
        }

        public void DeleteUsers(long[] ids)
        {
            var errors = Users.AsPredicateTrue()
                                   .And(x => ids.Contains(x.Id))
                                   .Evaluate()
                                   .ToList().MarkAsDeleted();

            foreach (var error in errors)
            {
                Errors.Add(new Tuple<string, string>(string.Empty, error));
            }
        }

        public void Dispose()
        {
            if (Database != null)
                Database.Dispose();
        }

        public int SaveChanges()
        {
            return Database.SaveChanges();
        }
    }
}