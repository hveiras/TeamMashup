using System.Web.Mvc;

namespace TeamMashup.Server.Areas.Private
{
    public class PrivateAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Private";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Private_project_plan",
                "Private/{tenant}/Projects/{projectname}/Plan/{controller}/{action}/{id}",
                new { controller = "Backlog", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Private_project_work",
                "Private/{tenant}/Projects/{projectname}/Work/{controller}/{action}/{id}",
                new { controller = "Work", action = "Board", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Private_project_reports",
                "Private/{tenant}/Projects/{projectname}/Reports/{controller}/{action}/{id}",
                new { controller = "Reports", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Private_users",
                "Private/{tenant}/Users/{user}/{action}",
                new { controller = "User", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
               "Private_accounts",
               "Private/{tenant}/Accounts/{userId}/{action}",
               new { controller = "Account", action = "Index", id = UrlParameter.Optional },
               new[] { "TeamMashup.Server.Areas.Private.Controllers" }
            );

            var route = context.MapRoute(
                "Private_default",
                "Private/{tenant}/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "TeamMashup.Server.Areas.Private.Controllers" }
            );

            route.DataTokens["area"] = "Private";
        }
    }
}