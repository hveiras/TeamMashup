using System.Web.Mvc;

namespace TeamMashup.Server.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                 "Admin_users",
                 "Admin/{tenant}/Users/{action}/{id}",
                 new { controller = "User", action = "Index", id = UrlParameter.Optional }
             );

            context.MapRoute(
                "Admin_projects",
                "Admin/{tenant}/Projects/{action}/{id}",
                new { controller = "Project", action = "Index", id = UrlParameter.Optional }
            );

            var route = context.MapRoute(
                "Admin_default",
                "Admin/{tenant}/{controller}/{action}/{id}",
                new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
            );

            route.DataTokens["area"] = "Admin";
        }
    }
}