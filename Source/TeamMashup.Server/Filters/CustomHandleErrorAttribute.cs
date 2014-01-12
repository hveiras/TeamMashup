using System;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace TeamMashup.Server.Filters
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
                return;

            //TODO: Log the exception here!

            var httpException = filterContext.Exception as HttpException;
            var action = "Http500";
            var statusCode = (int)HttpStatusCode.InternalServerError;

            if (httpException != null)
            {
                statusCode = httpException.GetHttpCode();
                switch (statusCode)
                {
                    case 403:
                        action = "Http403";
                        break;
                    case 404:
                        action = "Http404";
                        break;
                    default:
                        action = "Http500";
                        break;
                }
            }

            filterContext.ExceptionHandled = true;

            var response = filterContext.HttpContext.Response;
            response.Clear();
            response.TrySkipIisCustomErrors = true;

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var urlHelper = new UrlHelper(filterContext.HttpContext.Request.RequestContext);
                var message = filterContext.Exception.Message;
                HandleAjaxError(filterContext, message, urlHelper.Action(action, "Error", new { message = filterContext.HttpContext.Server.UrlEncode(message) }));
                return;
            }

            response.Redirect(String.Format("~/Error/{0}/?message={1}", action, filterContext.HttpContext.Server.UrlEncode(filterContext.Exception.Message)));
        }

        private static void HandleAjaxError(ExceptionContext filterContext, string message, string redirectUrl)
        {
            filterContext.Result = new JsonResult
            {
                Data = new { Success = false, Error = true, Message = message, RedirectUrl = redirectUrl },
                ContentEncoding = System.Text.Encoding.UTF8,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
}