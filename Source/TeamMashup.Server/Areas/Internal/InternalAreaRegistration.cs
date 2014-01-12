using System.Web.Mvc;

namespace TeamMashup.Server.Areas.Internal
{
    public class InternalAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Internal";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            var route = context.MapRoute(
                "Internal_default",
                "Internal/{controller}/{action}/{id}",
                new { controller = "Account", action = "Login", id = UrlParameter.Optional },
                new[] { "TeamMashup.Server.Areas.Internal.Controllers" }
            );
            
            route.DataTokens["area"] = "Internal";
        }
    }
}
