using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Web.Mvc;
using TeamMashup.Core.Domain;
using TeamMashup.Core.I18n;
using TeamMashup.Membership;
using TeamMashup.Models.Internal;

namespace TeamMashup.Server.Areas
{
    public class TenantBaseController : Controller
    {
        protected TenantContext Context { get; private set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var susbcriptionId = WebSecurity.CurrentUserSubscriptionId;
            Context = new TenantContext(susbcriptionId);

            ViewBag.UserName = Context.Users.GetById(WebSecurity.CurrentUserId).Name;

            var subscription = Context.Database.Subscriptions.GetById(susbcriptionId);
            ViewBag.TenantName = subscription.CompanyName;
            ViewBag.EmailDomain = subscription.EmailDomain;
            ViewBag.ShowChatCollapsed = true;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            if (Context != null)
                Context.Dispose();
        }

        protected virtual string RenderPartialView(string partialViewName, object model)
        {
            if (ControllerContext == null)
                return string.Empty;

            if (model == null)
                throw new ArgumentNullException("model");

            if (string.IsNullOrEmpty(partialViewName))
                throw new ArgumentNullException("partialViewName");

            ViewData.Model = model;//Set the model to the partial view

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, partialViewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }

        protected bool TryGetCurrentUser(long userId, out User user)
        {
            user = null;
            try
            {
                user = Context.Users.GetById(userId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected JsonResult JsonContent(bool success, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("content should not be null or empty.");

            return Json(new { success = success, Content = content });
        }

        protected JsonResult JsonSuccess()
        {
            return Json(new { Success = true });
        }

        protected JsonResult JsonSuccess(string redirectUrl)
        {
            return Json(new { Success = true, RedirectUrl = redirectUrl });
        }

        protected JsonResult JsonError(string message = "Sorry, an error has ocurred. If the error persists contact System Admninstrator.")
        {
            return Json(new { Success = false, Message = message });
        }

        protected JsonResult JsonView(bool success, string viewName, object model)
        {
            return Json(new { Success = success, View = RenderPartialView(viewName, model) });
        }

        protected void AddModelErrors(IList<Tuple<string, string>> errors)
        {
            foreach (var error in errors)
            {
                AddModelError(error.Item1, error.Item2);       
            }
        }

        protected void AddModelError(string key, string value)
        {
            var resourceManager = new ResourceManager(typeof(ErrorMessages));
            var localized = resourceManager.GetString(value);
            ModelState.AddModelError(key, localized);
        }

        public ActionResult ChatWindow(long userId, int openedCount)
        {
            User user;
            if (!Context.Database.Users.TryGetById(userId, out user))
                throw new InvalidOperationException("user not found!");

            ViewBag.OpenedCount = openedCount;

            var model = new UserChatModel
            {
                Id = user.Id,
                Name = user.Name
            };

            return PartialView("_ChatWindow", model);
        }

        [ChildActionOnly]
        public ActionResult ChatClient()
        {
            return PartialView("_ChatClient");
        }
    }
}