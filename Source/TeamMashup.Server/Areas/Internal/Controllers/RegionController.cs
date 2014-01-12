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
    public class RegionController : InternalBaseController
    {
        public ActionResult Countries()
        {
            return View();
        }

        public ActionResult Languages()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult NavTabs(string activeTab)
        {
            ViewBag.ActiveTab = activeTab ?? string.Empty;
            return PartialView("_NavTabs");
        }

        [ChildActionOnly]
        public ActionResult AddCountry()
        {
            return PartialView("_AddCountry");
        }

        [ChildActionOnly]
        public ActionResult AddLanguage()
        {
            return PartialView("_AddLanguage");
        }

        public ActionResult GetCountryItems(int iDisplayStart, int iDisplayLength, string sEcho)
        {
            var query = Context.Countries.OrderByDescending(x => x.Name);

            var totalRecords = query.Count();

            var page = query.Skip(iDisplayStart).Take(iDisplayLength).ToList();

            var model = new DataTablePage
            {
                sEcho = sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords
            };

            var countries = (from x in page
                             select new CountryModel
                             {
                                 Id = x.Id,
                                 Name = x.Name
                             }).ToList();

            foreach (var country in countries)
            {
                var item = new Dictionary<string, string>
                    {
                        {"Name", country.Name},
                        {"DT_RowId", "roleItem_" + country.Id}
                    };

                model.aaData.Add(item);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLanguageItems(int iDisplayStart, int iDisplayLength, string sEcho)
        {
            var query = Context.Languages.OrderByDescending(x => x.Name);

            var totalRecords = query.Count();

            var page = query.Skip(iDisplayStart).Take(iDisplayLength).ToList();

            var model = new DataTablePage
            {
                sEcho = sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords
            };

            var languages = (from x in page
                             select new LanguageModel
                             {
                                 Id = x.Id,
                                 Name = x.Name,
                                 Code = x.Code
                             }).ToList();

            foreach (var language in languages)
            {
                var item = new Dictionary<string, string>
                    {
                        {"Name", language.Name},
                        {"Code", language.Code},
                        {"DT_RowId", "roleItem_" + language.Id}
                    };

                model.aaData.Add(item);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [SetResponseStatus]
        public ActionResult AddCountry(CountryModel model)
        {
            if (ModelState.IsValid)
            {
                var country = new Country(model.Name);
                Context.AddCountry(country);

                Context.SaveChanges();
            }

            return JsonView(ModelState.IsValid,"_AddCountry", model);
        }

        [HttpPost]
        [SetResponseStatus]
        public ActionResult AddLanguage(LanguageModel model)
        {
            if (ModelState.IsValid)
            {
                var language = new Language(model.Code, model.Name);
                Context.AddLanguage(language);

                Context.SaveChanges();
            }

            return JsonView(ModelState.IsValid, "_AddLanguage", model);
        }

        public ActionResult EditCountry(long id)
        {
            Country country;
            if (!Context.Countries.TryGetById(id, out country))
                throw new InvalidOperationException(string.Format("country with id {0} was not found", id));

            var model = new CountryModel
            {
                Id = country.Id,
                Name = country.Name
            };

            return PartialView("_EditCountry", model);
        }

        public ActionResult EditLanguage(long id)
        {
            Language language;
            if (!Context.Languages.TryGetById(id, out language))
                throw new InvalidOperationException(string.Format("language with id {0} was not found", id));

            var model = new LanguageModel
            {
                Id = language.Id,
                Name = language.Name,
                Code = language.Code
            };

            return PartialView("_EditLanguage", model);
        }

        [HttpPost]
        public ActionResult EditCountry(CountryModel model)
        {
            if (ModelState.IsValid)
            {
                Country country;
                if (!Context.Countries.TryGetById(model.Id, out country))
                    throw new InvalidOperationException(string.Format("country with id {0} was not found", model.Id));

                country.Name = model.Name;
                Context.SaveChanges();
            }

            return JsonView(ModelState.IsValid, "_EditCountry", model);
        }

        [HttpPost]
        public ActionResult EditLanguage(LanguageModel model)
        {
            if (ModelState.IsValid)
            {
                Language language;
                if (!Context.Languages.TryGetById(model.Id, out language))
                    throw new InvalidOperationException(string.Format("language with id {0} was not found", model.Id));

                language.Name = model.Name;
                Context.SaveChanges();
            }

            return JsonView(ModelState.IsValid, "_EditLanguage", model);
        }

        public ActionResult DeleteCountries(long id)
        {
            return PartialView("_DeleteModalOld");
        }

        public ActionResult DeleteLanguages(long id)
        {
            return PartialView("_DeleteModalOld");
        }

        [HttpPost]
        public ActionResult DeleteCountries(long[] ids)
        {
            var countries = Context.Countries.FilterByIds(ids).ToList();
            var notDeletableCountries = countries.Where(x => Context.Subscriptions.Any(y => y.CountryId == x.Id)).ToList();

            var countriesToDelete = countries.Except(notDeletableCountries, new EntityComparer()).Cast<Country>().ToList();

            foreach (var country in countriesToDelete)
            {
                Context.Countries.Remove(country);
            }

            Context.SaveChanges();

            if (notDeletableCountries.Any())
                return JsonError("Some of the countries could not be deleted because they are in use.");

            return JsonSuccess();
        }

        [HttpPost]
        public ActionResult DeleteLanguages(long[] ids)
        {
            var languages = Context.Languages.FilterByIds(ids).ToList();
            var notDeletableLanguages = languages.Where(x => Context.UserProfiles.Any(y => y.LanguageId == x.Id)).ToList();

            var languagesToDelete = languages.Except(notDeletableLanguages, new EntityComparer()).Cast<Language>().ToList();

            foreach (var language in languagesToDelete)
            {
                Context.Languages.Remove(language);
            }

            Context.SaveChanges();

            if (notDeletableLanguages.Any())
                return JsonError("Some of the languges could not be deleted because they are in use.");

            return JsonSuccess();
        }
    }
}