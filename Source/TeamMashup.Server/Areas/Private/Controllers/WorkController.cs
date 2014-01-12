using Microsoft.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TeamMashup.Core;
using TeamMashup.Core.Contracts;
using TeamMashup.Core.Domain;
using TeamMashup.Core.Enums;
using TeamMashup.Membership;
using TeamMashup.Models.Private;
using TeamMashup.Server.Filters;

namespace TeamMashup.Server.Areas.Private.Controllers
{
    [NoCache]
    public class WorkController : ProjectBaseController
    {
        public ActionResult Board()
        {
            var activeIteration = ProjectContext.Iterations
                                         .AsPredicateTrue()
                                         .And(x => x.StateValue == (int)IterationState.Commited)
                                         .Evaluate()
                                         .SingleOrDefault();

            ViewBag.ShowBoard = activeIteration != null;

            var model = new BoardModel();

            if (activeIteration != null)
            {
                model.IterationId = activeIteration.Id;
                model.IterationName = activeIteration.Name;
            }

            return View(model);
        }

        public ActionResult GetIterationIssues(long iterationId, ScheduleState state, int iDisplayStart, int iDisplayLength, string sEcho)
        {
            var query = ProjectContext.Issues
                               .AsPredicateTrue()
                               .And(x => x.IterationId == iterationId)
                               .And(x => x.StateValue == (int)state)
                               .And(x => !x.Deleted)
                               .Evaluate()
                               .OrderByDescending(x => x.CreatedDate);

            var page = query.GetPage(iDisplayStart, iDisplayLength);

            var model = new DataTablePage
            {
                sEcho = sEcho,
                iTotalRecords = page.TotalItems,
                iTotalDisplayRecords = page.TotalItems
            };

            foreach (var issue in page.Items)
            {
                var item = new Dictionary<string, string>
                {
                    {"Summary", issue.Summary},
                    {"Reporter", issue.Reporter.Name},
                    {"Type", issue.Type.GetDescription()},
                    {"StoryPoints", issue.StoryPoints.ToString()},
                    {"Priority", issue.Priority.ToString()},
                    {"DT_RowId", "issue_" + issue.Id},
                    {"DT_RowClass", "row-" + issue.Type.ToString().ToLower()}
                };

                model.aaData.Add(item);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ChangeIssueState(long id, ScheduleState state)
        {
            Issue issue;
            if (!ProjectContext.Issues.TryGetById(id, out issue))
                throw new ApplicationException("issue with id = " + id + " was not found!");

            if (issue.Assignee == null || issue.Assignee.Id != WebSecurity.CurrentUserId)
                return JsonError("You can't change issue status because its unnasigned or assigned to another user");

            issue.State = state;

            ProjectContext.TrackIssueProgress(issue, IssueProgressType.StatusChanged);

            Context.SaveChanges();

            return JsonSuccess();
        }

        public ActionResult CompleteIteration(long id)
        {
            Iteration iteration;
            if (!ProjectContext.Iterations.TryGetById(id, out iteration))
                throw new ApplicationException("iteration with id = " + id + " was not found!");

            iteration.State = IterationState.Accepted;

            Context.SaveChanges();

            return this.RedirectToAction<BacklogController>(x => x.Index());
        }

        public ActionResult LogWork(long issueId)
        {
            Issue issue;
            if (!ProjectContext.Issues.TryGetById(issueId, out issue))
                throw new ApplicationException("issue with id = " + issueId + " was not found!");

            var model = new LogWorkModel
            {
                IssueId = issue.Id,
            };

            return PartialView("_LogWork", model);
        }

        [HttpPost]
        public ActionResult LogWork(LogWorkModel model)
        {
            Issue issue;
            if (!ProjectContext.Issues.TryGetById(model.IssueId, out issue))
                throw new ApplicationException("issue with id = " + model.IssueId + " was not found!");

            issue.RemainingEstimate = model.RemainingEstimate;
            issue.TimeSpent += model.TimeSpent;

            Context.SaveChanges();

            return JsonSuccess(Url.Action("Index", "Issue", new {id = model.IssueId}));
        }

        public ActionResult CloseIssue(long issueId)
        {
            Issue issue;
            if (!ProjectContext.Issues.TryGetById(issueId, out issue))
                throw new ApplicationException("issue with id = " + issueId + " was not found!");

            var model = new IssueModel
            {
                Id = issue.Id,
            };

            return PartialView("_CloseIssue", model);
        }

        [HttpPost]
        public ActionResult CloseIssue(IssueModel model)
        {
            Issue issue;
            if (!ProjectContext.Issues.TryGetById(model.Id, out issue))
                throw new ApplicationException("issue with id = " + model.Id + " was not found!");

            issue.State = ScheduleState.Done;

            ProjectContext.TrackIssueProgress(issue, IssueProgressType.StatusChanged);

            Context.SaveChanges();

            return JsonSuccess(Url.Action("Board", "Work"));
        }
    }
}