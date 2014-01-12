using System.Web.Mvc;
using System.Web.Routing;

namespace TeamMashup.Server
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var route = routes.MapRoute(
                 "Default", // Route name
                 "{controller}/{action}/{id}", // URL with parameters
                 new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                 new[] { "TeamMashup.Server.Areas.Public.Controllers" }
             );

            route.DataTokens["area"] = "Public";
        }
    }
}