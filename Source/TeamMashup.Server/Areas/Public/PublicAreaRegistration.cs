using System.Web.Mvc;

namespace TeamMashup.Server.Areas.Public
{
    public class PublicAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Public";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            var route = context.MapRoute(
                "Public_default",
                "Public/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "TeamMashup.Server.Areas.Public.Controllers" }
            );

            route.DataTokens["area"] = "Public";
        }
    }
}