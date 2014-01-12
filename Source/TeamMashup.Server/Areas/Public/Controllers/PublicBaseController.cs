using System.Web.Mvc;
using TeamMashup.Server.Filters;

namespace TeamMashup.Server.Areas.Public.Controllers
{
    [NoCache]
    public class PublicBaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.ShowLogin = false;
        }
    }
}
