using Microsoft.Web.Mvc;
using System;
using System.Linq;
using System.Resources;
using System.Web.Mvc;
using TeamMashup.Core.Domain;
using TeamMashup.Core.Enums;
using TeamMashup.Core.I18n;
using TeamMashup.Membership;
using TeamMashup.Models.Private;
using TeamMashup.Server.Filters;
using TeamMashup.Server.Models;

namespace TeamMashup.Server.Areas.Private.Controllers
{
    [NoCache]
    public class BacklogController : ProjectBaseController
    {
        public ActionResult Index()
        {
            return View("Issues");
        }

        [ChildActionOnly]
        public ActionResult AddIssueInline()
        {
            var model = new AddIssueInlineModel
            {
                Type = IssueType.UserStory
            };

            return PartialView("_AddIssueInline", model);
        }

        [HttpPost]
        [SetResponseStatus]
        public ActionResult AddIssueInline(AddIssueInlineModel model)
        {
            if (ModelState.IsValid)
            {
                Context.Database.Issues.Add(new Issue(model.Summary, model.Type, ProjectId, WebSecurity.CurrentUserId)
                {
                    State = ScheduleState.Backlog
                });

                Context.SaveChanges();
            }

            return JsonView(ModelState.IsValid, "_AddIssueInline", model);
        }

        public ActionResult AddIssue(IssueType type, string summary)
        {
            if (string.IsNullOrWhiteSpace(summary))
                summary = string.Empty;

            var model = new IssueModel
            {
                Type = type,
                Summary = summary
            };

            return PartialView("_AddIssue", model);
        }

        [HttpPost]
        [SetResponseStatus]
        public ActionResult AddIssue(IssueModel model)
        {
            if (ModelState.IsValid)
            {
                Context.Database.Issues.Add(new Issue(model.Summary, model.Type, ProjectId, model.ReporterId)
                {
                    State = ScheduleState.Backlog,
                    Description = model.Description,
                    ReporterId = model.ReporterId,
                    Priority = model.Priority,
                    StoryPoints = model.StoryPoints
                });

                Context.SaveChanges();
            }

            return JsonView(ModelState.IsValid, "_AddIssue", model);
        }

        public ActionResult Edit(long id)
        {
            Issue issue;
            if (!ProjectContext.Issues.TryGetById(id, out issue))
                throw new ApplicationException("issue with id = " + id + " was not found!");

            var model = new IssueModel
            {
                Id = issue.Id,
                Description = issue.Description,
                Summary = issue.Summary,
                ReporterId = issue.Reporter.Id,
                ReporterName = issue.Reporter.Name,
                Priority = issue.Priority,
                State = issue.State,
                StoryPoints = issue.StoryPoints,
                Tags = issue.Tags.ToList(),
                Type = issue.Type
            };

            return View("EditIssue", model);
        }

        [HttpPost]
        public ActionResult Edit(IssueModel model)
        {
            if (ModelState.IsValid)
            {
                Issue issue;
                if (!ProjectContext.Issues.TryGetById(model.Id, out issue))
                    throw new ApplicationException("issue with id = " + model.Id + " was not found!");

                var reporter = Context.Users.GetById(model.ReporterId);

                issue.Summary = model.Summary;
                issue.Description = model.Description;
                issue.Reporter = reporter;
                issue.Priority = model.Priority;
                issue.StoryPoints = model.StoryPoints;
                issue.Tags = model.Tags;
                issue.State = model.State;

                Context.SaveChanges();

                return this.RedirectToAction(x => x.Index());
            }

            return View("EditIssue", model);
        }

        public ActionResult Delete(long id)
        {
            Issue issue;
            if (!ProjectContext.Issues.TryGetById(id, out issue))
                throw new InvalidOperationException("The Issue you are trying to delete does not exist.");

            var resourceManager = new ResourceManager(typeof(Localized));
            var message = resourceManager.GetString("AreYouSureYouWantToDelete");

            var model = new DeleteModel
            {
                Ids = new[] { id },
                Message = string.Format("{0} {1}?", message, issue.Summary),
                Controller = "Backlog"
            };
            return PartialView("_DeleteModal", model);
        }

        [HttpPost]
        [SetResponseStatus]
        public ActionResult Delete(long[] ids)
        {
            ProjectContext.DeleteIssues(ids);
            Context.SaveChanges();

            if (ProjectContext.Errors.Any())
            {
                AddModelErrors(ProjectContext.Errors);
            }

            var model = new DeleteModel { Ids = ids, Message = string.Empty, Controller = "Backlog" };
            return JsonView(ModelState.IsValid && !ProjectContext.Errors.Any(), "_DeleteModal", model);
        }
    }
}