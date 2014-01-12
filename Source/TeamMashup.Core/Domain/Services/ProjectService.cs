using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamMashup.Core.Extensions;
using TeamMashup.Core.Persistence;

namespace TeamMashup.Core.Domain.Services
{
    public class ProjectService : IService
    {
        public IDatabaseContext Database { get; private set; }

        public long SubscriptionId { get; private set; }

        public ProjectService(long subscriptionId, IDatabaseContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            Database = context;
            SubscriptionId = subscriptionId;
        }

        public bool TryGetById(long id, out Project project)
        {
            return Database.Projects.TryGetById(id, out project);
        }

        public Project GetByName(string name)
        {
            return (from p in Database.Projects
                    where p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) &&
                          p.SubscriptionId == SubscriptionId
                    select p).Single();
        }

        public void Add(Project project)
        {
            Project existing;
            if (Database.Projects.TryGetByName(project.Name, out existing))
                throw new InvalidOperationException("Cannot add the Project with id: " + project.Id + ", the name is aleready in use");

            Database.Projects.Add(project);
        }

        public IQueryable<Project> All()
        {
            return Database.Projects.FilterBySubscription(SubscriptionId);
        }

        public IEnumerable<Project> GetMostRecentEditedProjects(int page, int pageSize, bool includeEnded = false)
        {
            var predicate = PredicateBuilder.True<Project>();
            predicate.And(x => x.SubscriptionId == SubscriptionId);

            if (!includeEnded)
                predicate.And(x => !x.IsEnded);

            var query = Database.Projects.AsExpandable().Where(predicate).OrderByDescending(x => x.ModifiedDate);

            return query.Paged<Project>(page, pageSize).ToList();
        }

        public IEnumerable<Project> SearchProjects(string projectName, bool includeEnded = false, int maxResults = 25)
        {
            var query = Database.Projects.Where(x => x.SubscriptionId == SubscriptionId);

            if (!includeEnded)
                query.Where(x => !x.IsEnded);

            return query.Where(x => x.Name.Contains(projectName)).Take(maxResults).ToList();
        }

        public IEnumerable<ProjectAssignmentRole> GetProjectAssignmentRoles()
        {
            return Database.ProjectAssignmentRoles.ToList();
        }

        public IEnumerable<Project> GetUserProjects(long userId)
        {
            var user = Database.Users.GetById(userId);

            var projectIds = (from x in user.Assignments
                              select x.ProjectId).ToList();

            return Database.Projects.FilterByIds(projectIds).ToList();
        }
    }
}