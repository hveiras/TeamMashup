using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using TeamMashup.Core.Domain;
using TeamMashup.Membership;
using TeamMashup.Server.App_Start;

namespace TeamMashup.Server
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteTable.Routes.MapHubs();
            AreaRegistration.RegisterAllAreas();

            // Use LocalDB for Entity Framework by default
            Database.DefaultConnectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\v11.0; Integrated Security=True; MultipleActiveResultSets=True");

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            using (var context = new DatabaseContext())
            {
                UserProfile profile = null;
                User user;
                if (WebSecurity.TryGetCurrentUser(out user))
                    profile = context.UserProfiles.SingleOrDefault(x => x.UserId == user.Id);

                CultureInfo culture;
                if (profile == null)
                    culture = new CultureInfo("en");
                else
                    culture = new CultureInfo(profile.Language.Code);

                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
                return;

            var route = RouteTable.Routes.GetRouteData(new HttpContextWrapper(Context));
            if (route == null || route.Route.GetType().Name == "IgnoreRouteInternal")
                return;

            if (!(Context.User.Identity is FormsIdentity))
                return;

            //TODO: evaluate refactor this for a simple .ContainsValue("Public")
            if (!route.DataTokens.ContainsValue("Private") || !route.DataTokens.ContainsValue("Admin") || !route.DataTokens.ContainsValue("Subscription"))
                return;

            var currentTenant = route.GetRequiredString("tenant");

            var id = (FormsIdentity)Context.User.Identity;
            var userTenant = id.Ticket.UserData;
            if (userTenant.Trim().ToLower() != currentTenant.Trim().ToLower())
            {
                FormsAuthentication.SignOut();
                Response.Redirect("/");
            }
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                var serializer = new JavaScriptSerializer();

                var serializeModel = serializer.Deserialize<WebPrincipalSerializeModel>(authTicket.UserData);

                var newUser = new WebPrincipal(authTicket.Name)
                {
                    Id = serializeModel.Id,
                    SusbcriptionId = serializeModel.SusbcriptionId
                };

                HttpContext.Current.User = newUser;
            }
        }
    }
}