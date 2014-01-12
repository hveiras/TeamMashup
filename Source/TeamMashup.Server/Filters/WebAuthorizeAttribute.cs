using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TeamMashup.Core.Domain;
using TeamMashup.Membership;

namespace TeamMashup.Server.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class WebAuthorizeAttribute : AuthorizeAttribute
    {
        public string Claims { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (!filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated)
                return;

            //Verify if the user is associated to a valid account.
            using (var context = new DatabaseContext())
            {
                var userEmail = filterContext.RequestContext.HttpContext.User.Identity.Name;
                var user = context.Users.SingleOrDefault(x => x.Email.Equals(userEmail, StringComparison.InvariantCultureIgnoreCase));

                if (user == null)
                {
                    FormsAuthentication.SignOut();
                    HttpContext.Current.User = (IPrincipal)new GenericPrincipal(new GenericIdentity(string.Empty), new string[] { });
                }
            }

            //Verify if the user is authorized.
            string[] claims;
            if (!string.IsNullOrEmpty(Claims))
            {
                claims = Claims.Split(',');
                if (!WebSecurity.AccessControl.UserHasClaims(WebSecurity.CurrentUserId, claims))
                    throw new HttpException(403, "Unauthorized");
            }
        }
    }
}