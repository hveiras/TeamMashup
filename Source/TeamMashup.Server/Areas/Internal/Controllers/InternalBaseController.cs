using System;
using System.IO;
using System.Web.Mvc;
using TeamMashup.Core.Domain;
using TeamMashup.Membership;
using TeamMashup.Models;
using TeamMashup.Models.Internal;
using TeamMashup.Server.Filters;

namespace TeamMashup.Server.Areas.Internal.Controllers
{
    [NoCache]
    public class InternalBaseController : Controller
    {
        protected const int PageDefault = 1;
        protected const int PageSizeDefault = 25;
        protected IDatabaseContext Context;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Context = new DatabaseContext();
            ViewBag.ShowChatCollapsed = true;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            if (Context != null)
                Context.Dispose();
        }

        [ChildActionOnly]
        public ActionResult AccountMenu()
        {
            User user;
            if (!WebSecurity.TryGetCurrentUser(out user))
                throw new InvalidOperationException("user is not authenticated!");

            var model = new UserModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
            };

            return PartialView("_AccountMenu", model);
        }

        public ActionResult ChatWindow(long userId, int openedCount)
        {
            User user;
            if (!Context.Users.TryGetById(userId, out user))
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

        protected JsonResult JsonContent(bool success, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("content should not be null or empty.");

            return Json (new { success = success, Content = content });
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
            return Json(new { Success = false, Error = message });
        }

        protected JsonResult JsonView(bool success, string viewName, object model)
        {
            return Json(new { Success = success, View = RenderPartialView(viewName, model)});
        }
    }
}
