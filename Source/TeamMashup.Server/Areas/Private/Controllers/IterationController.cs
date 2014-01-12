using Microsoft.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web.Mvc;
using TeamMashup.Core;
using TeamMashup.Core.Contracts;
using TeamMashup.Core.Domain;
using TeamMashup.Core.I18n;
using TeamMashup.Models.Private;
using TeamMashup.Server.Filters;
using TeamMashup.Server.Models;

namespace TeamMashup.Server.Areas.Private.Controllers
{
    [NoCache]
    public class IterationController : ProjectBaseController
    {
        public ActionResult Index(long? releaseId)
        {
            if (releaseId.HasValue)
            {
                ViewBag.ReleaseId = releaseId.Value;
            }
            else
            {
                long id;
                if (TryGetMostRecentSelectableReleaseId(out id))
                    ViewBag.ReleaseId = id;
            }

            return View("Iterations");
        }

        [ChildActionOnly]
        public ActionResult AddIterationInline(long releaseId)
        {
            ViewBag.ReleaseId = releaseId;
            return PartialView("_AddIterationInline");
        }

        [HttpPost]
        [SetResponseStatus]
        public ActionResult AddIterationInline(IterationModel model)
        {
            if (!ModelState.IsValid) return JsonView(ModelState.IsValid, "_AddIterationInline", model);

            if (ProjectContext.TryAddIteration(model))
            {
                Context.SaveChanges();
            }
            else
            {
                AddModelErrors(ProjectContext.Errors);
            }

            return JsonView(ModelState.IsValid, "_AddIterationInline", model);
        }

        public ActionResult GetIterations(long releaseId, int iDisplayStart, int iDisplayLength, string sEcho)
        {
            var query = ProjectContext.Iterations
                               .AsPredicateTrue()
                               .And(x => x.ReleaseId == releaseId)
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

            foreach (var item in page.Items.Select(iteration => new Dictionary<string, string>
            {
                {"Name", iteration.Name },
                {"State", iteration.State.GetDescription() },
                {"From", iteration.From.ToShortDateString() },
                {"To", iteration.To.ToShortDateString() },
                {"DT_RowId", "release_" + iteration.Id}
            }))
            {
                model.aaData.Add(item);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(long id)
        {
            Iteration iteration;
            if (!ProjectContext.Iterations.TryGetById(id, out iteration))
                throw new ApplicationException("iteration with id = " + id + " was not found!");

            var model = new IterationModel
            {
                Id = iteration.Id,
                Name = iteration.Name,
                Description = iteration.Description,
                From = iteration.From.Date,
                To = iteration.To.Date,
                State = iteration.State,
                ReleaseId = iteration.ReleaseId
            };

            return View("EditIteration", model);
        }

        [HttpPost]
        public ActionResult Edit(IterationModel model)
        {
            if (!ModelState.IsValid) return View("EditIteration", model);

            if (ProjectContext.TryEditIteration(model))
            {
                Context.SaveChanges();
                return this.RedirectToAction(x => x.Index(model.ReleaseId));
            }

            AddModelErrors(ProjectContext.Errors);
            return View("EditIteration", model);
        }

        public ActionResult Delete(long id)
        {
            Iteration iteration;
            if(!ProjectContext.Iterations.TryGetById(id, out iteration))
                throw new InvalidOperationException("The Iteration you are trying to delete does not exist.");

            var resourceManager = new ResourceManager(typeof(Localized));
            var message = resourceManager.GetString("AreYouSureYouWantToDelete");

            var model = new DeleteModel 
            { 
                Ids = new[] { id },
                Message = string.Format("{0} {1}?", message, iteration.Name),
                Controller = "Iteration" 
            };
            return PartialView("_DeleteModal", model);
        }

        [HttpPost]
        [SetResponseStatus]
        public ActionResult Delete(long[] ids)
        {
            ProjectContext.DeleteIterations(ids);
            Context.SaveChanges();

            if (ProjectContext.Errors.Any())
            {
                AddModelErrors(ProjectContext.Errors);
            }

            var model = new DeleteModel { Ids = ids, Message = string.Empty, Controller = "Iteration" };
            return JsonView(ModelState.IsValid && !ProjectContext.Errors.Any(), "_DeleteModal", model);
        }

        public ActionResult Resources(long? releaseId, long? iterationId)
        {
            var model = new IterationResourcesModel();

            if (!releaseId.HasValue)
            {
                long rId;
                if (TryGetMostRecentSelectableReleaseId(out rId, true))
                    releaseId = rId;
            }

            if (releaseId.HasValue)
                ViewBag.ReleaseId = releaseId.Value;

            if (releaseId.HasValue && !iterationId.HasValue)
            {
                long iId;
                if (TryGetMostRecentSelectableIterationId(releaseId.Value, out iId))
                    iterationId = iId;
            }

            if (iterationId.HasValue)
            {
                ViewBag.IterationId = iterationId.Value;

                var pc = ProjectContext;

                Iteration iteration;
                if (!pc.Iterations.TryGetById(iterationId.Value, out iteration))
                    throw new ApplicationException("Itertion with id = " + iterationId + " does not exist");

                model.StoryPointValue = iteration.StoryPointValueInHours;
                model.IterationId = iterationId.Value;

                var resources = pc.GetIterationResources(iteration.Id);

                //TODO: Implement a service for the pictures and retrieve the proper size based on the requirements.
                const long pictureId = Constants.InvalidId;
                model.Resources = resources.Select(x => new IterationResourceModel
                {
                    Id = x.Item2.Id,
                    UserId = x.Item1.Id,
                    PictureId = pictureId,
                    UserName = x.Item1.Name,
                    Capacity = x.Item2.Capacity,
                    Velocity = x.Item2.Velocity
                }).ToList();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateResources(IterationResourcesModel model)
        {
            var pc = ProjectContext;

            Project project;
            if (!Context.Projects.TryGetById(ProjectId, out project))
                throw new ApplicationException("Project with id = " + ProjectId + " does not exist");

            Iteration iteration;
            if (!pc.Iterations.TryGetById(model.IterationId, out iteration))
                throw new ApplicationException("Iteration with id = " + model.IterationId + " does not exist");

            if (ModelState.IsValid)
            {
                iteration.StoryPointValueInHours = model.StoryPointValue;

                foreach (var assignment in project.AssignmentsNonDeleted)
                {
                    var resource = model.Resources.SingleOrDefault(x => x.UserId == assignment.UserId);

                    if (resource != null)
                    {
                        if (!resource.Id.IsValidId()) //We create a new resource
                        {
                            assignment.IterationResources.Add(new IterationResource
                            {
                                Capacity = resource.Capacity,
                                Velocity = resource.Velocity,
                                IterationId = model.IterationId,
                                ProjectAssignmentId = assignment.Id
                            });
                        }
                        else
                        {
                            var toBeUpdated = assignment.IterationResources.SingleOrDefault(x => x.Id == resource.Id);
                            toBeUpdated.Capacity = resource.Capacity;
                            toBeUpdated.Velocity = resource.Velocity;
                            toBeUpdated.IterationId = model.IterationId;
                            toBeUpdated.ProjectAssignmentId = assignment.Id;
                        }
                    }
                }

                Context.SaveChanges();

                return this.RedirectToAction(x => x.Resources(iteration.ReleaseId, iteration.Id));
            }

            ViewBag.ReleaseId = iteration.ReleaseId;
            ViewBag.IterationId = iteration.Id;
            return View("Resources", model);
        }
    }
}