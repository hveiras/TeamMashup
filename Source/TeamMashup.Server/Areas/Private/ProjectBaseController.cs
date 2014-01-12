using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TeamMashup.Core;
using TeamMashup.Core.Contracts;
using TeamMashup.Core.Domain;
using TeamMashup.Membership;
using TeamMashup.Server.Filters;

namespace TeamMashup.Server.Areas.Private
{
    [NoCache]
    public class ProjectBaseController : TenantBaseController
    {
        protected ProjectContext ProjectContext { get; private set; }

        protected long ProjectId { get; private set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var projectName = RouteData.Values["projectname"].ToString();

            projectName = Server.UrlDecode(projectName);
            var project = Context.Projects.GetByName(projectName);
            ViewBag.ProjectName = project.Name;

            ProjectContext = Context.Project(project.Id);
            ProjectId = project.Id;

            if (!ProjectContext.Members.Any(x => x.UserId == WebSecurity.CurrentUserId))
                throw new HttpException((int)HttpStatusCode.Unauthorized, "you don't have permissions to work on this project");
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            if (ProjectContext != null)
                ProjectContext.Dispose();
        }

        public ActionResult GetBacklogItems(int iDisplayStart, int iDisplayLength, string sEcho)
        {
            int totalRecords;
            var stories = ProjectContext.GetBacklogItems(iDisplayStart, iDisplayLength, out totalRecords).ToList();

            var model = new DataTablePage
            {
                sEcho = sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords
            };

            foreach (var story in stories)
            {
                var item = new Dictionary<string, string>
                {
                    {"Summary", story.Summary},
                    {"Reporter", story.Reporter.Name},
                    {"Type", story.Type.GetDescription()},
                    {"StoryPoints", story.StoryPoints.ToString()},
                    {"Priority", story.Priority.ToString()},
                    {"DT_RowId", "backlogItem_" + story.Id}
                };

                model.aaData.Add(item);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public ActionResult SelectRelease(long releaseId, string action, string controller, bool onlyWithIterations = false)
        {
            IList<Release> releases;
            if (onlyWithIterations)
            {
                releases = GetPlanningOrActiveReleases().ToList()
                           .Where(r => r.IterationsNonDeleted.Any()).ToList();
            }
            else
                releases = GetPlanningOrActiveReleases().ToList();

            if (releases.Any())
            {
                var release = releases.Single(x => x.Id == releaseId);
                var index = releases.IndexOf(release);
                releases.RemoveAt(index);
                releases.Insert(0, release); //We position the selected release first.
            }

            var model = (from r in releases select new SelectListItem { Value = r.Id.ToString(), Text = r.Name }).ToList();
            
            if (model.Any())
                model.First().Selected = true;

            ViewBag.Action = action;
            ViewBag.Controller = controller;
            return PartialView("_SelectRelease", model);
        }

        [ChildActionOnly]
        public ActionResult SelectIteration(long releaseId, long iterationId, string action, string controller)
        { 
            Release release;
            if (!ProjectContext.Releases.TryGetById(releaseId, out release))
                throw new ApplicationException("Release with id = " + releaseId + " does not exist");

            var model = (from i in release.Iterations
                         let selected = i.Id == iterationId
                         select new SelectListItem { Value = i.Id.ToString(), Text = i.Name, Selected = selected }).ToList();

            ViewBag.Action = action;
            ViewBag.Controller = controller;
            ViewBag.ReleaseId = releaseId;
            return PartialView("_SelectIteration", model);
        }

        [ChildActionOnly]
        public ActionResult NavTabs(string activeTab)
        {
            ViewBag.ActiveTab = activeTab ?? string.Empty;
            return PartialView("_NavTabs");
        }

        [ChildActionOnly]
        public ActionResult NavPills(string tab, string activePill)
        {
            ViewBag.Tab = tab;
            ViewBag.ActivePill = activePill ?? string.Empty;
            return PartialView("_NavPills");
        }

        public ActionResult SearchUsers(string searchTerm)
        {
            var model = (from x in ProjectContext.Members
                         let user = Context.Users.GetById(x.UserId)
                         where user.Name.RemoveDiacritics().IndexOf(searchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 && !user.Deleted
                         select new TypeaheadDatum
                         {
                             value = user.Id,
                             name = user.Name
                         }).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        protected bool TryGetMostRecentSelectableReleaseId(out long releaseId, bool mustHaveIterations = false)
        {
            releaseId = 0;
            try
            {
                IEnumerable<Release> releases;

                if (mustHaveIterations)
                {
                    releases = GetPlanningOrActiveReleases().ToList().Where(r => r.IterationsNonDeleted.Any()).ToList();
                }
                else
                {
                    releases = GetPlanningOrActiveReleases().ToList();
                }

                if (!releases.Any())
                    return false;

                releaseId = releases.First().Id;
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected bool TryGetMostRecentSelectableIterationId(long releaseId, out long iterationId)
        {
            iterationId = 0;
            try
            {
                var iterations = GetPlanningOrCommitedIterations(releaseId);
                if (!iterations.Any())
                    return false;

                iterationId = iterations.First().Id;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private IQueryable<Release> GetPlanningOrActiveReleases()
        {
            return ProjectContext
                           .GetReleases()
                           .PlanningOrActive();
        }

        private IEnumerable<Iteration> GetPlanningOrCommitedIterations(long releaseId)
        {
            return ProjectContext.Iterations
                          .AsPredicateTrue()
                          .And(x => x.ReleaseId == releaseId)
                          .Evaluate()
                          .ToList()
                          .PlanningOrCommited();
        }
    }
}
