using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TeamMashup.Core.Contracts;
using TeamMashup.Core.Enums;

namespace TeamMashup.Core.Domain
{
    public static partial class EntityExtensions
    {
        #region Generic Extensions

        public static bool TryGetById<T>(this IQueryable<T> entities, long id, out T entity) where T : class, IEntitySet
        {
            entity = null;

            if (!id.IsValidId())
                return false;

            try
            {
                entity = entities.Single(x => x.Id == id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryGetByName<T>(this IQueryable<T> entities, string name, out T entity) where T : class, IUniqueNamedEntity
        {
            entity = null;

            try
            {
                entity = entities.GetByName(name);
                return entity != null;
            }
            catch
            {
                return false;
            }
        }

        public static T GetById<T>(this IQueryable<T> entities, long id) where T : class, IEntitySet
        {
            return entities.SingleOrDefault(x => x.Id == id);
        }

        public static IQueryable<T> FilterByIds<T>(this IQueryable<T> entities, IEnumerable<long> ids) where T : class, IEntitySet
        {
            return entities.Where(x => ids.Contains(x.Id));
        }

        public static T GetByName<T>(this IQueryable<T> entities, string name) where T : class, IUniqueNamedEntity
        {
            return entities.SingleOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool Exists<T>(this IQueryable<T> entities, string name) where T : class, IUniqueNamedEntity
        {
            return GetByName(entities, name) != null;
        }

        public static IEnumerable<T> GetByNames<T>(this IQueryable<T> entities, IEnumerable<string> names) where T : class, IUniqueNamedEntity
        {
            var query = from name in names
                        join entity in entities on name equals entity.Name
                        select entity;

            return query.ToList();
        }

        public static IQueryable<T> NonDeleted<T>(this IQueryable<T> entities) where T : class, IDeletableEntity
        {
            return entities.Where(x => !x.Deleted);
        }

        public static IEnumerable<T> NonDeleted<T>(this IEnumerable<T> entities) where T : class, IDeletableEntity
        {
            return entities.Where(x => !x.Deleted).ToList();
        }

        public static IEnumerable<string> MarkAsDeleted<T>(this IEnumerable<T> entities) where T : class, IDeletableEntity
        {
            var errors = new List<string>();

            foreach (var entity in entities)
            {
                string errorKey;
                if (!entity.TryDelete(out errorKey))
                    errors.Add(errorKey);
            }

            return errors;
        }

        public static IQueryable<T> FilterBySubscription<T>(this IQueryable<T> entities, long subscriptionId) where T : class, ISubscriptionQueryable
        {
            return entities.Where(x => x.SubscriptionId == subscriptionId);
        }

        public static PagedResult<T> GetPage<T>(this IQueryable<T> entities, int start, int lenght) where T : class, IEntitySet
        {
            return new PagedResult<T>
            {
                Items = entities.Skip(start).Take(lenght).ToList(),
                TotalItems = entities.Count()
            };
        }

        public static PredicateQuery<T> AsPredicateTrue<T>(this IQueryable<T> query) where T : class
        {
            return new PredicateQuery<T>(query, PredicateBuilder.True<T>());
        }

        public static PredicateQuery<T> And<T>(this PredicateQuery<T> predicate, Expression<Func<T, bool>> condition) where T : class
        {
            predicate.Predicate = predicate.Predicate.And(condition);

            return predicate;
        }

        public static IQueryable<T> Evaluate<T>(this PredicateQuery<T> predicate) where T : class
        {
            return predicate.Query.AsExpandable().Where(predicate.Predicate);
        }

        public static bool AnyOverlapsWith(this IQueryable<IScheduleableEntity> entities, IScheduleableEntity entity)
        {
            return entities.Any(x => x.From < entity.To && x.To > entity.From);
        }

        #endregion Generic Extensions

        public static IQueryable<Claim> PlatformClaims(this IQueryable<Claim> claims)
        {
            return claims.Where(x => x.ScopeValue == (int)AssetScope.Platform);
        }

        public static IQueryable<Claim> TenantClaims(this IQueryable<Claim> claims)
        {
            return claims.Where(x => x.ScopeValue == (int)AssetScope.Tenant);
        }

        public static bool TryGetActive(this IQueryable<Iteration> iterations, out Iteration iteration)
        {
            iteration = null;

            try
            {
                iteration = iterations.Where(x => x.StateValue == (int)IterationState.Commited).Single();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static IEnumerable<Iteration> PlanningOrCommited(this IEnumerable<Iteration> iterations)
        {
            return iterations.Where(x => x.StateValue == (int)IterationState.Planning || x.StateValue == (int)IterationState.Commited);
        }

        public static IQueryable<Log> Search(this IQueryable<Log> logs, int start, int lenght, string searchTerm, out int totalResults)
        {
            var predicate = PredicateBuilder.True<Log>();
            predicate = predicate.And(x => !x.Deleted);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                predicate = predicate.And(x => x.Message.Contains(searchTerm));
            }

            var query = logs.AsExpandable().Where(predicate).OrderByDescending(x => x.CreatedDate);

            totalResults = query.Count();

            return query.Skip(start).Take(lenght);
        }

        public static bool TryGetByCode(this IQueryable<PasswordRecovery> recoveries, Guid token, out PasswordRecovery recovery)
        {
            recovery = null;

            if (token == Guid.Empty)
                return false;

            try
            {
                recovery = recoveries.Single(x => x.Code == token);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static IQueryable<Project> Search(this IQueryable<Project> projects, int start, int lenght, string searchTerm, out int totalResults)
        {
            var predicate = PredicateBuilder.True<Project>();
            predicate = predicate.And(x => !x.Deleted);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                predicate = predicate.And(x => x.Name.Contains(searchTerm));
            }

            var query = projects.AsExpandable().Where(predicate).OrderByDescending(x => x.ModifiedDate);

            totalResults = query.Count();

            return query.Skip(start).Take(lenght);
        }

        public static IQueryable<Release> PlanningOrActive(this IQueryable<Release> releases)
        {
            return releases.Where(x => x.StateValue == (int)ReleaseState.Planning || x.StateValue == (int)ReleaseState.Active);
        }

        public static IQueryable<Role> PlatformRoles(this IQueryable<Role> roles)
        {
            return roles.Where(x => x.ScopeValue == (int)AssetScope.Platform);
        }

        public static IQueryable<Role> TenantRoles(this IQueryable<Role> roles)
        {
            return roles.Where(x => x.ScopeValue == (int)AssetScope.Tenant);
        }

        public static IQueryable<Subscription> InActiveState(this IQueryable<Subscription> subscriptions)
        {
            return subscriptions.Where(x => x.State == SubscriptionState.Active);
        }

        public static IQueryable<Subscription> InPaymentFailedState(this IQueryable<Subscription> subscriptions)
        {
            return subscriptions.Where(x => x.State == SubscriptionState.PaymentFailed);
        }

        public static IQueryable<Subscription> InPastDueState(this IQueryable<Subscription> subscriptions)
        {
            return subscriptions.Where(x => x.State == SubscriptionState.PastDue);
        }

        public static bool TryGetBySubscriptor(this IQueryable<Subscription> subscriptions, User subscriptor, out Subscription subscription)
        {
            subscription = null;
            try
            {
                subscription = subscriptions.Single(x => x.Subscriptor.Id == subscriptor.Id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryGetByTenantName(this IQueryable<Subscription> subscriptions, string tenantName, out Subscription subscription)
        {
            subscription = null;
            try
            {
                subscription = subscriptions.Single(x => x.TenantName.Equals(tenantName, StringComparison.InvariantCultureIgnoreCase));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static IQueryable<SubscriptionPlanHistory> GetBySubscription(this IQueryable<SubscriptionPlanHistory> history, Subscription subscription)
        {
            return history.Where(x => x.SubscriptionId == subscription.Id);
        }

        public static bool TryGetByEmail(this IQueryable<User> users, string email, out User user)
        {
            user = null;
            try
            {
                email = email.Trim();
                user = users.Single(x => x.Email == email && !x.Deleted);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static IQueryable<User> PlatformUsers(this IQueryable<User> users)
        {
            return users.Where(x => x.SubscriptionId == Constants.InvalidId);
        }

        public static IQueryable<Issue> ByProject(this IQueryable<Issue> userStories, long projectId)
        {
            return userStories.Where(x => x.ProjectId == projectId);
        }

        internal static IQueryable<Issue> ByRelease(this IQueryable<Issue> userStories, long releaseId)
        {
            return userStories.Where(x => x.ReleaseId == releaseId);
        }

        internal static IQueryable<Issue> ByIteration(this IQueryable<Issue> userStories, long iterationId)
        {
            return userStories.Where(x => x.IterationId == iterationId);
        }

        public static IQueryable<Subscription> GetSubscriptionsToBill(this IQueryable<Subscription> subscriptions, int max)
        {
            return (from x in subscriptions
                    let endDate = x.CurrentPeriodEndDate.HasValue ? x.CurrentPeriodEndDate : DateTime.MinValue
                    where (x.StateValue == (int)SubscriptionState.Active ||
                    x.StateValue == (int)SubscriptionState.PaymentFailed ||
                    x.StateValue == (int)SubscriptionState.PastDue) &&
                    endDate <= DateTime.UtcNow
                    orderby x.CurrentPeriodEndDate ascending
                    select x).Take(max);
        }

        public static bool TryGet(this IQueryable<SystemSetting> settings, string key, out SystemSetting setting)
        {
            setting = null;

            try
            {
                setting = settings.Where(x => x.Key == key).Single();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryGetNext(this IQueryable<BackupRequest> requests, out BackupRequest request)
        {
            request = null;

            try
            {
                var query = from r in requests
                            where r.StateValue == (int)BackupState.Pending
                            orderby r.CreatedDate
                            select r;

                request = query.FirstOrDefault();

                return request != null;
            }
            catch
            {
                return false;
            }
        }
    }
}