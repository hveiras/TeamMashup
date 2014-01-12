using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using TeamMashup.Core.Domain;
using TeamMashup.Core.Enums;
using TeamMashup.Models;
using TeamMashup.Models.Admin;
using TeamMashup.Models.Private;

namespace TeamMashup.Server.Areas.Private.Controllers
{
    public class ReportsController : ProjectBaseController
    {
        public ActionResult Index()
        {
            var model = new BurndownModel();

            Iteration iteration;
            var anIterationIsActive = ProjectContext.Iterations.TryGetActive(out iteration);

            ViewBag.ShowReports = anIterationIsActive;

            if (anIterationIsActive)
            {
                var startOfIteration = iteration.From.Date;
                var endOfIteration = iteration.To.Date;

                var issueProgress = (from x in ProjectContext.IssueProgresses
                                     where x.IterationId == iteration.Id
                                     select x).ToList();

                var startOfIterationStoryPoints = (from x in issueProgress
                                                   where x.Type == IssueProgressType.IterationStarted
                                                   select x).Sum(x => x.StoryPoints);

                var issueProgressByDay = from x in issueProgress
                                         group x by x.Date.Date into g
                                         select new { Date = g.Key, Changes = g.ToList() };

                var previousDayStoryPoints = 0;
                var storyPointsByDay = new Dictionary<DateTime, int> {{startOfIteration, startOfIterationStoryPoints}};

                foreach (var item in issueProgressByDay)
                {
                    previousDayStoryPoints = previousDayStoryPoints + item.Changes.Sum(x => x.StoryPoints);
                    storyPointsByDay.Add(item.Date.AddDays(1), previousDayStoryPoints);
                }

                var builder = new StringBuilder();
                foreach (var sp in storyPointsByDay)
                {
                    builder.Append(string.Format("[{0},{1}],", sp.Key.GetMillisecondsFromEpoch(), sp.Value));
                }

                var start = startOfIteration.GetMillisecondsFromEpoch();
                var end = endOfIteration.GetMillisecondsFromEpoch();

                model.DataSets = new List<DataSet>
                {
                    new DataSet 
                    { 
                        Key="ideal",
                        Label = "Ideal",
                        Data = string.Format("[[{0},{1}],[{2},{3}]]", start, startOfIterationStoryPoints, end, 0)
                    },
                    new DataSet 
                    { 
                        Key="open",
                        Label = "Open Story Points",
                        Data = string.Format("[{0}]",
                        builder.ToString().TrimEnd(','))
                    }
                };
            }


            return View("Burndown", model);
        }

        public ActionResult Iteration(long? releaseId, long? iterationId)
        {
            var model = new IterationReportModel();
            ViewBag.ShowReports = false;

            Iteration iteration;
            var anIterationIsActive = ProjectContext.Iterations.TryGetActive(out iteration);

            if (!releaseId.HasValue)
            {
                long rId;
                if (anIterationIsActive)
                {
                    releaseId = iteration.ReleaseId;
                }
                else
                {
                    if (TryGetMostRecentSelectableReleaseId(out rId, mustHaveIterations: true))
                        releaseId = rId;
                }
            }

            if (releaseId.HasValue)
                ViewBag.ReleaseId = releaseId.Value;

            if (releaseId.HasValue && !iterationId.HasValue)
            {
                long iId;
                if (anIterationIsActive)
                {
                    iterationId = iteration.Id;
                }
                else
                {
                    if (TryGetMostRecentSelectableIterationId(releaseId.Value, out iId))
                        iterationId = iId;
                }
            }

            if (iterationId.HasValue)
            {
                ViewBag.IterationId = iterationId.Value;
                ViewBag.ShowReports = true;

                model.IterationId = iterationId.Value;

                var issues = ProjectContext.GetIterationIssues(iterationId.Value);

                model.Commited = issues.Sum(x => x.StoryPoints);
                model.Completed = (from x in issues
                                   where x.State == ScheduleState.Done
                                   select x.StoryPoints).Sum();

                var issuesByUser = (from x in issues
                                    where x.Assignee != null
                                    group x by x.Assignee into g
                                    select g).ToList();

                model.UserStats = (from x in issuesByUser
                                   select new Tuple<UserModel, IterationIssueStats>(new UserModel
                                   {
                                       Id = x.Key.Id,
                                       Name = x.Key.Name,
                                   },
                                   new IterationIssueStats
                                   {
                                       DefinedPoints = (from y in x
                                                        where y.State == ScheduleState.Defined
                                                        select y.StoryPoints).Sum(),
                                       DonePoints = (from y in x
                                                     where y.State == ScheduleState.Done
                                                     select y.StoryPoints).Sum(),
                                       InProgressPoints = (from y in x
                                                           where y.State == ScheduleState.InProgress
                                                           select y.StoryPoints).Sum()
                                   })).ToList();
            }

            return View("Iteration", model);
        }
    }
}
