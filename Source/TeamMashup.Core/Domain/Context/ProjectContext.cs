using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using TeamMashup.Core.Enums;
using TeamMashup.Models.Private;

namespace TeamMashup.Core.Domain
{
    public class ProjectContext : IDisposable, IContext
    {
        public IList<Tuple<string, string>> Errors { get; private set; }

        public IDatabaseContext Database { get; private set; }

        public long ProjectId { get; private set; }

        public ProjectContext(IDatabaseContext context, long projectId)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            Database = context;
            ProjectId = projectId;
            Errors = new List<Tuple<string, string>>();
        }

        public IQueryable<Issue> Issues
        {
            get
            {
                return Database.Issues.Where(x => x.ProjectId == ProjectId);
            }
        }

        public IQueryable<IssueProgress> IssueProgresses
        {
            get
            {
                return Database.IssueProgresses.Where(x => x.ProjectId == ProjectId);
            }
        }

        public IQueryable<Release> Releases
        {
            get
            {
                return Database.Releases.Where(x => x.ProjectId == ProjectId && !x.Deleted);
            }
        }

        public IQueryable<Iteration> Iterations
        {
            get
            {
                return Database.Iterations.Where(x => x.ProjectId == ProjectId && !x.Deleted);
            }
        }

        public IEnumerable<ProjectAssignment> Members
        {
            get
            {
                var project = Database.Projects.GetById(ProjectId);
                return project.AssignmentsNonDeleted;
            }
        }

        public bool TryAddRelease(ReleaseModel model)
        {
            var release = new Release(model.Name, model.From, model.To, ProjectId)
            {
                Description = model.Description
            };

            if (Releases.AnyOverlapsWith(release))
            {
                Errors.Add(new Tuple<string, string>("from", "ReleaseOverlapedErrorKey"));
                Errors.Add(new Tuple<string, string>("to", "ReleaseOverlapedErrorKey"));

                return false;
            }

            Database.Releases.Add(release);

            return true;
        }

        public bool TryAddIteration(IterationModel model)
        {
            var iteration = new Iteration(model.Name, model.From, model.To, ProjectId, model.ReleaseId)
            {
                Description = model.Description
            };

            if (Iterations.AnyOverlapsWith(iteration))
            {
                Errors.Add(new Tuple<string, string>("from", "IterationOverlapedErrorKey"));
                Errors.Add(new Tuple<string, string>("to", "IterationOverlapedErrorKey"));

                return false;
            }

            Release release;
            if (!Releases.TryGetById(iteration.ReleaseId, out release))
                throw new InvalidOperationException("iteration with id " + iteration.Id + "does not belong to any Release");

            if (iteration.From < release.From || iteration.To > release.To)
            {
                Errors.Add(new Tuple<string, string>("from", "IterationOutsideTheBoundsOfRelease"));
                Errors.Add(new Tuple<string, string>("to", "IterationOutsideTheBoundsOfRelease"));

                return false;
            }

            Database.Iterations.Add(iteration);

            return true;
        }

        public bool TryEditRelease(ReleaseModel model)
        {
            Release release;
            if (!Releases.TryGetById(model.Id, out release))
                throw new ApplicationException("release with id = " + model.Id + " was not found!");

            release.Name = model.Name;
            release.Description = model.Description;
            release.From = model.From;
            release.To = model.To;
            release.ProjectId = ProjectId;

            var releases = Releases.AsPredicateTrue()
                                   .And(x => x.Id != release.Id)
                                   .Evaluate();

            if (!releases.AnyOverlapsWith(release)) return true;

            Errors.Add(new Tuple<string, string>("from", "ReleaseOverlapedErrorKey"));
            Errors.Add(new Tuple<string, string>("to", "ReleaseOverlapedErrorKey"));

            return false;
        }

        public bool TryEditIteration(IterationModel model)
        {
            Iteration iteration;
            if (!Iterations.TryGetById(model.Id, out iteration))
                throw new ApplicationException("iteration with id = " + model.Id + " was not found!");

            iteration.Name = model.Name;
            iteration.Description = model.Description;
            iteration.From = model.From;
            iteration.To = model.To;
            iteration.ProjectId = ProjectId;

            var iterations = Iterations.AsPredicateTrue()
                                   .And(x => x.Id != iteration.Id)
                                   .Evaluate();

            if (iterations.AnyOverlapsWith(iteration))
            {
                Errors.Add(new Tuple<string, string>("from", "IterationOverlapedErrorKey"));
                Errors.Add(new Tuple<string, string>("to", "IterationOverlapedErrorKey"));

                return false;
            }

            Release release;
            if (!Releases.TryGetById(iteration.ReleaseId, out release))
                throw new InvalidOperationException("iteration with id " + iteration.Id + "does not belong to any Release");

            if (iteration.From < release.From || iteration.To > release.To)
            {
                Errors.Add(new Tuple<string, string>("from", "IterationOutsideTheBoundsOfRelease"));
                Errors.Add(new Tuple<string, string>("to", "IterationOutsideTheBoundsOfRelease"));

                return false;
            }

            return true;
        }

        public void DeleteReleases(long[] ids)
        {
            var errors = Releases.AsPredicateTrue()
                                   .And(x => ids.Contains(x.Id))
                                   .Evaluate()
                                   .ToList().MarkAsDeleted();

            foreach (var error in errors)
            {
                Errors.Add(new Tuple<string, string>(string.Empty, error));
            }
        }

        public void DeleteIterations(long[] ids)
        {
            var errors = Iterations.AsPredicateTrue()
                                   .And(x => ids.Contains(x.Id))
                                   .Evaluate()
                                   .ToList().MarkAsDeleted();

            foreach (var error in errors)
            {
                Errors.Add(new Tuple<string, string>(string.Empty, error));
            }
        }

        public void DeleteIssues(long[] ids)
        {
            var errors = Issues.AsPredicateTrue()
                                   .And(x => ids.Contains(x.Id))
                                   .Evaluate()
                                   .ToList().MarkAsDeleted();

            foreach (var error in errors)
            {
                Errors.Add(new Tuple<string, string>(string.Empty, error));
            }
        }


        public void TrackIssueProgress(IEnumerable<Issue> issues, IssueProgressType type)
        {
            foreach (var issue in issues)
            {
                TrackIssueProgress(issue, type);
            }
        }

        public void TrackIssueProgress(Issue issue, IssueProgressType type)
        {
            var lastChange = (from x in IssueProgresses
                              where x.IssueId == issue.Id
                              orderby x.Date descending
                              select x).FirstOrDefault();

            int storyPoints;

            if (lastChange != null)
            {
                switch (lastChange.State)
                {
                    case ScheduleState.Defined:
                        switch (issue.State)
                        {
                            case ScheduleState.Done:
                                storyPoints = (-1) * issue.StoryPoints;
                                break;
                            default:
                                return;
                        }
                        break;
                    case ScheduleState.InProgress:
                        switch (issue.State)
                        {
                            case ScheduleState.Done:
                                storyPoints = (-1) * issue.StoryPoints;
                                break;
                            default:
                                return;
                        }
                        break;
                    case ScheduleState.Done:
                        switch (issue.State)
                        {
                            case ScheduleState.InProgress:
                                storyPoints = issue.StoryPoints;
                                break;
                            case ScheduleState.Defined:
                                storyPoints = issue.StoryPoints;
                                break;
                            default:
                                return;
                        }
                        break;
                    default:
                        return;
                }
            }
            else
            {
                storyPoints = issue.StoryPoints;
            }

            var change = new IssueProgress
            {
                IssueId = issue.Id,
                IterationId = issue.IterationId,
                ProjectId = issue.ProjectId,
                ReleaseId = issue.ReleaseId,
                State = issue.State,
                StoryPoints = storyPoints,
                AssigneeId = issue.AssigneeId,
                Date = DateTime.UtcNow,
                Type = type
            };

            Database.IssueProgresses.Add(change);
        }

        public IEnumerable<IssueProgress> GetIssueChanges(long issueId)
        {
            return IssueProgresses.Where(x => x.IssueId == issueId).ToList();
        }

        public IEnumerable<Issue> GetIterationIssues(long iterationId)
        {
            return Issues.Where(x => x.IterationId == iterationId).ToList();
        }

        /// <summary>
        /// Get all active User Stories in backlog state.
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public IEnumerable<Issue> GetBacklogItems(int displayStart, int displayLenght, out int totalRecords)
        {
            var predicate = PredicateBuilder.True<Issue>();
            predicate = predicate.And(x => !x.Deleted);
            predicate = predicate.And(x => x.ProjectId == ProjectId);
            predicate = predicate.And(x => x.StateValue == (int)ScheduleState.Backlog);

            var query = Database.Issues.AsExpandable().Where(predicate).OrderByDescending(x => x.CreatedDate);

            totalRecords = query.Count();

            return query.Skip(displayStart).Take(displayLenght).ToList();
        }

        [Obsolete]
        public IQueryable<Release> GetReleases()
        {
            var predicate = PredicateBuilder.True<Release>();
            predicate = predicate.And(x => !x.Deleted);
            predicate = predicate.And(x => x.ProjectId == ProjectId);

            var query = Database.Releases.AsExpandable().Where(predicate).OrderByDescending(x => x.CreatedDate);

            return query;
        }

        /// <summary>
        /// Returuns all the resources related to a given Iteration.
        /// </summary>
        /// <param name="iterationId">The id of the Iteration</param>
        /// <returns></returns>
        [Obsolete]
        public IList<Tuple<User, IterationResource>> GetIterationResources(long iterationId)
        {
            Project project;
            if (!Database.Projects.TryGetById(ProjectId, out project))
                throw new ApplicationException("GetIterationResources failed: project with id " + ProjectId + " does not exist");

            return (from assignment in project.AssignmentsNonDeleted
                let user = Database.Users.GetById(assignment.UserId)
                let resource = assignment.IterationResources.SingleOrDefault(x => x.IterationId == iterationId) ?? new IterationResource
                {
                    IterationId = iterationId, Capacity = 0, Velocity = 0, ProjectAssignmentId = assignment.Id
                }
                select new Tuple<User, IterationResource>(user, resource)).ToList();
        }

        /// <summary>
        /// Returns the available Story Points of an Iteration based on its Resources.
        /// </summary>
        /// <param name="iterationId">The id of the Iteration</param>
        /// <returns></returns>
        [Obsolete]
        public int GetAvailableStoryPoints(long iterationId)
        {
            var tuples = GetIterationResources(iterationId);

            return tuples.Sum(tuple => tuple.Item2.Capacity);
        }

        [Obsolete]
        public bool TryScheduleUserStories(IList<long> userStoryIds, long releaseId, long iterationId)
        {
            var stories = Database.Issues.FilterByIds(userStoryIds).ToList();

            if (!stories.Any())
                return false;

            Release release;
            if (!Database.Releases.TryGetById(releaseId, out release))
                return false;

            Iteration iteration;
            if (!Database.Iterations.TryGetById(iterationId, out iteration))
                return false;

            foreach (var story in stories)
            {
                story.ReleaseId = release.Id;
                story.IterationId = iteration.Id;
                story.State = ScheduleState.Defined;
            }

            Database.SaveChanges();

            return true;
        }

        [Obsolete]
        public bool TryUnscheduleUserStories(IList<long> userStoryIds, long iterationId)
        {
            var stories = Database.Issues.FilterByIds(userStoryIds).ToList();

            if (!stories.Any())
                return false;

            Iteration iteration;
            if (!Database.Iterations.TryGetById(iterationId, out iteration))
                return false;

            foreach (var story in stories)
            {
                iteration.UserStories.Remove(story);
                story.State = ScheduleState.Backlog;
            }

            Database.SaveChanges();

            return true;
        }

        public int SaveChanges()
        {
            return Database.SaveChanges();
        }

        public void Dispose()
        {
            if (Database != null)
                Database.Dispose();
        }
    }
}
