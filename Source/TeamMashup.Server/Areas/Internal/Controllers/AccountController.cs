using Microsoft.Web.Mvc;
using System.Web.Mvc;
using TeamMashup.Core;
using TeamMashup.Core.Tracking;
using TeamMashup.Membership;
using TeamMashup.Models.Internal;
using TeamMashup.Server.Filters;

namespace TeamMashup.Server.Areas.Internal.Controllers
{
    [WebAuthorize]
    [NoCache]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (WebSecurity.IsAuthenticated)
                return this.RedirectToAction<DashboardController>(x => x.Index());

            return View(new LoginModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.Email, model.Password, model.RememberMe))
            {
                LogHelper.Info(string.Format(LogConstants.LoginSuccessful, model.Email));
                return this.RedirectToAction<DashboardController>(x => x.Index());
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            WebSecurity.Logout();

            return this.RedirectToAction<AccountController>(x => x.Login());
        }

    }
}
