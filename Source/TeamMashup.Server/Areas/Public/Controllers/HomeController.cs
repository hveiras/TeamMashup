using System.Linq;
using System.Web.Mvc;
using TeamMashup.Core;
using TeamMashup.Core.Domain;
using TeamMashup.Core.Tracking;
using TeamMashup.Membership;
using TeamMashup.Models.Public;
using TeamMashup.Server.Filters;

namespace TeamMashup.Server.Areas.Public.Controllers
{
    [NoCache]
    public class HomeController : PublicBaseController
    {
        public ActionResult Index()
        {
            ViewBag.ShowLogin = true;
            return View("Home");
        }
        
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                if (WebSecurity.Login(model.Email, model.Password, model.RememberMe))
                {
                    var userId = WebSecurity.CurrentUserId;

                    using (var context = new DatabaseContext())
                    {
                        var subscription = context.Subscriptions.Single(x => x.Id == WebSecurity.CurrentUserSubscriptionId);

                        LogHelper.Info(string.Format(LogConstants.LoginSuccessful, model.Email));
                        return Redirect("~/Private/" + subscription.TenantName);
                    }
                }
                else
                {
                    LogHelper.Info(string.Format(LogConstants.LoginFailed, model.Email));
                    ModelState.AddModelError("email", "Invalid Email or Password");
                    ModelState.AddModelError("password", "Invalid Email or Password");
                }
            }

            ViewBag.ShowLogin = true;
            ViewBag.Email = model.Email;

            return View("Home");
        }

    }
}