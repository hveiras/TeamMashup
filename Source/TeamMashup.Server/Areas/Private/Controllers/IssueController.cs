using System;
using System.Web.Mvc;
using TeamMashup.Core.Domain;
using TeamMashup.Membership;
using TeamMashup.Models.Private;
using TeamMashup.Server.Filters;

namespace TeamMashup.Server.Areas.Private.Controllers
{
    [NoCache]
    public class IssueController : ProjectBaseController
    {
        public ActionResult Index(long id, string backUrl)
        {
            Issue issue;
            if (!ProjectContext.Issues.TryGetById(id, out issue))
                throw new ApplicationException("issue with id = " + id + " was not found!");

            var model = new IssueModel
            {
                Id = issue.Id,
                Summary = issue.Summary,
                Description = issue.Description,
                ReporterId = issue.Reporter.Id,
                ReporterName = issue.Reporter.Name,
                Priority = issue.Priority,
                State = issue.State,
                StoryPoints = issue.StoryPoints,
                Tags = issue.Tags,
                Type = issue.Type,
                AssigneeId = issue.AssigneeId,
                AssigneeName = issue.Assignee != null ? issue.Assignee.Name : "Not Assigned",
                TimeSpent = issue.TimeSpent,
                RemainingEstimate = issue.RemainingEstimate.HasValue ? issue.RemainingEstimate.ToString() : "Not Specified"
            };

            if (!string.IsNullOrEmpty(backUrl))
                ViewData.SetBackLink(Server.UrlDecode(backUrl));

            return View("Issue", model);
        }

        [HttpPost]
        public ActionResult AssignToMe(long id, string returnUrl)
        {
            Issue issue;
            if (!ProjectContext.Issues.TryGetById(id, out issue))
                throw new ApplicationException("issue with id = " + id + " was not found!");

            issue.AssigneeId = WebSecurity.CurrentUserId;

            Context.SaveChanges();

            return JsonSuccess(returnUrl);
        }
    }
}