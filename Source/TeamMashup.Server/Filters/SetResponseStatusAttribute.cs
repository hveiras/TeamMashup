using System.Web.Mvc;

namespace TeamMashup.Server.Filters
{
    public class SetResponseStatusAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                filterContext.HttpContext.Response.SetStatusCode(HttpStatusCodeExtended.UnprocessableEntity);
            }
        }
    }
}