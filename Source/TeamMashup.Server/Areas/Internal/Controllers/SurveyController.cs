using Microsoft.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TeamMashup.Core.Contracts;
using TeamMashup.Core.Domain;
using TeamMashup.Models.Internal;
using TeamMashup.Server.Filters;

namespace TeamMashup.Server.Areas.Internal.Controllers
{
    [NoCache]
    public class SurveyController : InternalBaseController
    {
        public ActionResult Surveys()
        {
            return View();
        }

        public ActionResult GetSurveyItems(int iDisplayStart, int iDisplayLength, string sEcho)
        {
            var query = Context.Surveys.OrderByDescending(x => x.CreatedDate);

            var totalRecords = query.Count();

            var page = query.Skip(iDisplayStart).Take(iDisplayLength).ToList();

            var model = new DataTablePage
            {
                sEcho = sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords
            };

            var surveys = (from x in page
                           select new SurveyModel
                           {
                               Id = x.Id,
                               Title = x.Title
                           }).ToList();

            foreach (var survey in surveys)
            {
                var item = new Dictionary<string, string>
                    {
                        {"Title", survey.Title},
                        {"DT_RowId", "roleItem_" + survey.Id}
                    };

                model.aaData.Add(item);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSurveyOptionsItems(long surveyId, int iDisplayStart, int iDisplayLength, string sEcho)
        {
            Survey survey;
            if(!Context.Surveys.TryGetById(surveyId, out survey))
                throw new InvalidOperationException("survey with id = " + surveyId + " was not found!");

            var query = survey.Options.OrderByDescending(x => x.Votes);

            var totalRecords = query.Count();

            var page = query.Skip(iDisplayStart).Take(iDisplayLength).ToList();

            var model = new DataTablePage
            {
                sEcho = sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords
            };

            var options = (from x in page
                           select new SurveyItemModel
                           {
                               Id = x.Id,
                               Description = x.Description,
                               Votes = x.Votes
                           }).ToList();

            foreach (var option in options)
            {
                var item = new Dictionary<string, string>
                    {
                        {"Description", option.Description},
                        {"Votes", option.Votes.ToString()},
                        {"DT_RowId", "roleItem_" + option.Id}
                    };

                model.aaData.Add(item);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public ActionResult AddSurvey()
        {
            return PartialView("_AddSurvey");
        }

        [ChildActionOnly]
        public ActionResult AddSurveyOption(long surveyId)
        {
            Survey survey;
            if (!Context.Surveys.TryGetById(surveyId, out survey))
                throw new InvalidOperationException(string.Format("survey with id {0} was not found", surveyId));

            var model = new SurveyItemModel
            {
                SurveyId = survey.Id
            };

            return PartialView("_AddSurveyOption", model);
        }

        [HttpPost]
        [SetResponseStatus]
        public ActionResult AddSurvey(SurveyModel model)
        {
            if (ModelState.IsValid)
            {
                var survey = new Survey(model.Title);
                Context.AddSurvey(survey);

                Context.SaveChanges();
            }

            return JsonView(ModelState.IsValid, "_AddSurvey", model);
        }

        [HttpPost]
        [SetResponseStatus]
        public ActionResult AddSurveyOption(SurveyItemModel model)
        {
            if (ModelState.IsValid)
            {
                Survey survey;
                if (!Context.Surveys.TryGetById(model.SurveyId, out survey))
                    throw new InvalidOperationException(string.Format("survey with id {0} was not found", model.SurveyId));

                var surveyOption = new SurveyItem(model.SurveyId, model.Description);

                survey.Options.Add(surveyOption);

                Context.SaveChanges();
            }

            return JsonView(ModelState.IsValid, "_AddSurveyOption", model);
        }

        public ActionResult Edit(long id)
        {
            Survey survey;
            if (!Context.Surveys.TryGetById(id, out survey))
                throw new InvalidOperationException(string.Format("survey with id {0} was not found", id));

            var model = new SurveyModel
            {
                Id = survey.Id,
                Title = survey.Title,
                Active = survey.Active
            };

            return View("EditSurvey", model);
        }

        public ActionResult EditSurveyOption(long id)
        {
            SurveyItem surveyItem;
            if (!Context.SurveyItems.TryGetById(id, out surveyItem))
                throw new InvalidOperationException(string.Format("survey item with id {0} was not found", id));

            var model = new SurveyItemModel
            {
                Id = surveyItem.Id,
                SurveyId = surveyItem.SurveyId,
                Description = surveyItem.Description,
                Votes = surveyItem.Votes
            };

            return PartialView("_EditSurveyOption", model);
        }

        [HttpPost]
        public ActionResult Edit(SurveyModel model)
        {
            if (ModelState.IsValid)
            {
                Survey survey;
                if (!Context.Surveys.TryGetById(model.Id, out survey))
                    throw new InvalidOperationException(string.Format("survey with id {0} was not found", model.Id));

                survey.Title = model.Title;
                Context.SaveChanges();
            }

            return View("EditSurvey", model);
        }

        [HttpPost]
        public ActionResult EditSurveyOption(SurveyItemModel model)
        {
            if (ModelState.IsValid)
            {
                SurveyItem surveyItem;
                if (!Context.SurveyItems.TryGetById(model.Id, out surveyItem))
                    throw new InvalidOperationException(string.Format("survey item with id {0} was not found", model.Id));

                surveyItem.Description = model.Description;
                Context.SaveChanges();
            }

            return JsonView(ModelState.IsValid, "_EditSurveyOption", model);
        }

        public ActionResult DeleteSurveys(long id)
        {
            return PartialView("_DeleteModalOld");
        }

        public ActionResult DeleteSurveyOptions(long id)
        {
            return PartialView("_DeleteModalOld");
        }

        [HttpPost]
        public ActionResult DeleteSurveys(long[] ids)
        {
            var surveys = Context.Surveys.FilterByIds(ids).ToList();
            var notDeletableSurveys = surveys.Where(x => x.Active).ToList();

            var surveysToDelete = surveys.Except(notDeletableSurveys, new EntityComparer()).Cast<Survey>().ToList();

            foreach (var survey in surveysToDelete)
            {
                Context.Surveys.Remove(survey);
            }

            Context.SaveChanges();

            if (notDeletableSurveys.Any())
                return JsonError("Some of the surveys could not be deleted because they are in use.");

            return JsonSuccess();
        }

        [HttpPost]
        public ActionResult DeleteSurveyOptions(long[] ids)
        {
            var surveyItems = Context.SurveyItems.FilterByIds(ids).ToList();

            var notDeletableSurveyItems = (from x in surveyItems
                                           let survey = Context.Surveys.GetById(x.SurveyId)
                                           let active = survey == null ? false : survey.Active
                                           where active
                                           select x).ToList();

            var surveyItemsToDelete = surveyItems.Except(notDeletableSurveyItems, new EntityComparer()).Cast<SurveyItem>().ToList();

            foreach (var surveyItem in surveyItemsToDelete)
            {
                Context.SurveyItems.Remove(surveyItem);
            }

            Context.SaveChanges();

            if (notDeletableSurveyItems.Any())
                return JsonError("Some of the survey items could not be deleted because they are in use.");

            return JsonSuccess();
        }

        public ActionResult Start(long surveyId)
        {
            Survey survey;
            if (!Context.Surveys.TryGetById(surveyId, out survey))
                throw new InvalidOperationException(string.Format("survey with id {0} was not found", surveyId));

            survey.Active = true;

            Context.SaveChanges();

            return this.RedirectToAction<SurveyController>(x => x.Surveys());
        }

        public ActionResult Finish(long surveyId)
        {
            Survey survey;
            if (!Context.Surveys.TryGetById(surveyId, out survey))
                throw new InvalidOperationException(string.Format("survey with id {0} was not found", surveyId));

            survey.Active = false;

            Context.SaveChanges();

            return this.RedirectToAction<SurveyController>(x => x.Surveys());
        }
    }
}