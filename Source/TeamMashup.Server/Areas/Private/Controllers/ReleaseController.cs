using Microsoft.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web.Mvc;
using TeamMashup.Core;
using TeamMashup.Core.Contracts;
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
    public class ReleaseController : ProjectBaseController
    {
        public ActionResult Index()
        {
            return View("Releases");
        }

        [ChildActionOnly]
        public ActionResult AddReleaseInline()
        {
            return PartialView("_AddReleaseInline");
        }

        [HttpPost]
        [SetResponseStatus]
        public ActionResult AddReleaseInline(ReleaseModel model)
        {
            if (!ModelState.IsValid) return JsonView(ModelState.IsValid, "_AddReleaseInline", model);

            if (ProjectContext.TryAddRelease(model))
            {
                Context.SaveChanges();
            }
            else
            {
                AddModelErrors(ProjectContext.Errors);
            }

            return JsonView(ModelState.IsValid, "_AddReleaseInline", model);
        }

        public ActionResult GetReleases(int iDisplayStart, int iDisplayLength, string sEcho)
        {
            var query = ProjectContext.Releases
                               .AsPredicateTrue()
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

            foreach (var release in page.Items)
            {
                var item = new Dictionary<string, string>
                {
                    {"Name", release.Name },
                    {"State", release.State.GetDescription() },
                    {"From", release.From.ToShortDateString() },
                    {"To", release.To.ToShortDateString() },
                    {"DT_RowId", "release_" + release.Id}
                };

                model.aaData.Add(item);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(long id)
        {
            Release release;
            if (!ProjectContext.Releases.TryGetById(id, out release))
                throw new ApplicationException("release with id = " + id + " was not found!");

            var model = new ReleaseModel
            {
                Id = release.Id,
                Name = release.Name,
                Description = release.Description,
                From = release.From.Date,
                To = release.To.Date,
                State = release.State
            };

            return View("EditRelease", model);
        }

        [HttpPost]
        public ActionResult Edit(ReleaseModel model)
        {
            if (!ModelState.IsValid) return View("EditRelease", model);

            if (ProjectContext.TryEditRelease(model))
            {
                Context.SaveChanges();
                return this.RedirectToAction(x => x.Index());
            }
            
            AddModelErrors(ProjectContext.Errors);
            return View("EditRelease", model);
        }

        public ActionResult Delete(long id)
        {
            Release release;
            if (!ProjectContext.Releases.TryGetById(id, out release))
                throw new InvalidOperationException("The Release you are trying to delete does not exist.");

            var resourceManager = new ResourceManager(typeof(Localized));
            var message = resourceManager.GetString("AreYouSureYouWantToDelete");

            var model = new DeleteModel
            {
                Ids = new[] { id },
                Message = string.Format("{0} {1}?", message, release.Name),
                Controller = "Release"
            };
            return PartialView("_DeleteModal", model);
        }

        [HttpPost]
        [SetResponseStatus]
        public ActionResult Delete(long[] ids)
        {
            ProjectContext.DeleteReleases(ids);
            Context.SaveChanges();

            if (ProjectContext.Errors.Any())
            {
                AddModelErrors(ProjectContext.Errors);
            }

            var model = new DeleteModel { Ids = ids, Message = string.Empty, Controller = "Release" };
            return JsonView(ModelState.IsValid && !ProjectContext.Errors.Any(), "_DeleteModal", model);
        }

        public ActionResult Plan(long? releaseId)
        {
            var model = new PlanReleaseModel();

            if (!releaseId.HasValue)
            {
                long rId;
                if (TryGetMostRecentSelectableReleaseId(out rId, mustHaveIterations: true))
                    releaseId = rId;
            }

            if (releaseId.HasValue)
            {
                var pc = ProjectContext;

                ViewBag.ReleaseId = releaseId.Value;

                Release release;
                if (!pc.Releases.TryGetById(releaseId.Value, out release))
                    throw new ApplicationException("Release with id = " + releaseId.Value + " was not found!");

                model.Name = release.Name;
                model.StartDate = release.From;
                model.EndDate = release.To;

                //TODO: Get Iteration Statuses as list and pass them into the model
                foreach (var iteration in release.IterationsNonDeleted)
                {
                    var maxStoryPoints = pc.GetAvailableStoryPoints(iteration.Id);
                    var storyPoints = iteration.UserStoriesNonDeleted.Sum(x => x.StoryPoints);

                    model.Iterations.Add(new PlanIterationModel
                    {
                        Id = iteration.Id,
                        ReleaseId = iteration.ReleaseId,
                        Name = iteration.Name,
                        State = iteration.State,
                        MaxStoryPoints = maxStoryPoints,
                        StoryPoints = storyPoints,
                        HasUserStories = iteration.UserStoriesNonDeleted.Any()
                    });
                }

                model.HasActiveIterations = release.IterationsNonDeleted.PlanningOrCommited().Any();

                ViewBag.ProjectIsActive = ProjectContext.Iterations
                                         .AsPredicateTrue()
                                         .And(x => x.StateValue == (int)IterationState.Commited)
                                         .Evaluate()
                                         .Any();
            }

            return View(model);
        }

        public ActionResult GetIterationIssues(long iterationId, int iDisplayStart, int iDisplayLength, string sEcho)
        {
            var pc = ProjectContext;

            Iteration iteration;
            if (!pc.Iterations.TryGetById(iterationId, out iteration))
                throw new ApplicationException("Iteration with id = " + iterationId + " was not found!");

            int totalRecords = iteration.UserStoriesNonDeleted.Count;
            var model = new DataTablePage
            {
                sEcho = sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords
            };

            foreach (var story in iteration.UserStoriesNonDeleted)
            {
                var item = new Dictionary<string, string>
                {
                    {"Name", story.Summary},
                    {"Reporter", story.Reporter.Name },
                    {"Type", story.Type.ToString()},
                    {"StoryPoints", story.StoryPoints.ToString()},
                    {"Priority", story.Priority.ToString()},
                    {"DT_RowId", "backlogItem_" + story.Id}
                };

                model.aaData.Add(item);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserStory(long userStoryId)
        {
            using (var context = new TenantContext(WebSecurity.CurrentUserSubscriptionId))
            {
                Issue userStory;
                if (!context.Database.Issues.TryGetById(userStoryId, out userStory))
                {
                    //TODO: Log
                    return Json(new JsonResponse<IssueModel> { Success = false });
                }

                var response = new JsonResponse<IssueModel>
                {
                    Data = new IssueModel
                    {
                        Id = userStory.Id,
                        Summary = userStory.Summary,
                        Description = userStory.Description
                    },
                    Success = true
                };

                return Json(response);
            }
        }

        [HttpPost]
        public ActionResult ChangeIterationState(long iterationId, IterationState state)
        {

            using (var context = new TenantContext(WebSecurity.CurrentUserSubscriptionId))
            {
                var db = context.Database;

                Iteration iteration;
                if (!db.Iterations.TryGetById(iterationId, out iteration))
                {
                    //TODO: Log
                    return Json(false);
                }

                iteration.State = state;
                db.SaveChanges();

                return Json(true);
            }

        }

        public ActionResult Configure(long releaseId, long iterationId)
        {
            return this.RedirectToAction<IterationController>(x => x.Resources(releaseId, iterationId));
        }


        [HttpPost]
        public ActionResult ScheduleIssues(long[] ids, long releaseId, long iterationId)
        {

            var pc = ProjectContext;

            if (!pc.TryScheduleUserStories(ids, releaseId, iterationId))
                return JsonError();

            return JsonSuccess();

        }

        [HttpPost]
        public ActionResult UnscheduleIssues(long[] ids, long iterationId)
        {

            var pc = ProjectContext;

            if (!pc.TryUnscheduleUserStories(ids, iterationId))
                return JsonError();

            return JsonSuccess();
        }

        public ActionResult StartIteration(long id)
        {
            Iteration iteration;
            if (!ProjectContext.Iterations.TryGetById(id, out iteration))
                throw new ApplicationException("iteration with id = " + id + " was not found!");

            var model = new IterationModel
            {
                Id = iteration.Id,
                Name = iteration.Name,
            };

            return PartialView("_StartIteration", model);
        }

        [HttpPost]
        public ActionResult StartIteration(IterationModel model)
        {
            Iteration iteration;
            if (!ProjectContext.Iterations.TryGetById(model.Id, out iteration))
                throw new ApplicationException("iteration with id = " + model.Id + " was not found!");

            iteration.State = IterationState.Commited;
            iteration.Release.State = ReleaseState.Active;

            var issues = ProjectContext.Issues.Where(x => x.IterationId == iteration.Id).ToList();

            ProjectContext.TrackIssueProgress(issues, IssueProgressType.IterationStarted);

            Context.SaveChanges();

            return JsonView(ModelState.IsValid, "_StartIteration", model);
        }

        public ActionResult MarkReleaseAsCompleted(long id)
        {
            Release release;
            if (!ProjectContext.Releases.TryGetById(id, out release))
                throw new ApplicationException("release with id = " + id + " was not found!");

            release.State = ReleaseState.Accepted;

            Context.SaveChanges();

            return this.RedirectToAction<ReleaseController>(x => x.Plan(null));
        }
    }
}