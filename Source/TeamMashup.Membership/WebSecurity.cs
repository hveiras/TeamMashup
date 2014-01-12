using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using TeamMashup.Core;
using TeamMashup.Core.Domain;

namespace TeamMashup.Membership
{
    public static class WebSecurity
    {
        const string Provider_Name = "WebMembershipProvider";

        public static long CurrentUserId
        {
            get { return ((WebPrincipal)Context.User).Id; }
        }

        public static string CurrentUserEmail
        {
            get { return ((WebPrincipal)Context.User).Email; }
        }

        public static long CurrentUserSubscriptionId
        {
            get { return ((WebPrincipal)Context.User).SusbcriptionId; }
        }

        public static bool HasUserId
        {
            get { return CurrentUserId > Constants.InvalidId; }
        }

        public static bool IsAuthenticated
        {
            get { return Request.IsAuthenticated; }
        }

        internal static HttpContextBase Context
        {
            get { return new HttpContextWrapper(HttpContext.Current); }
        }

        internal static HttpRequestBase Request
        {
            get { return Context.Request; }
        }

        internal static HttpResponseBase Response
        {
            get { return Context.Response; }
        }

        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "Login is used more consistently in ASP.Net")]
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "This is a helper class, and we are not removing optional parameters from methods in helper classes")]
        public static bool Login(string email, string password, bool persistCookie = false)
        {
            bool success = System.Web.Security.Membership.ValidateUser(email, password);
            if (success)
            {
                var provider = System.Web.Security.Membership.Providers[Provider_Name] as WebMembershipProvider;

                if(provider == null)
                    throw new InvalidCastException(Provider_Name);

                var user = provider.GetUser(email);

                var serializeModel = new WebPrincipalSerializeModel
                {
                    Email = user.Email,
                    Id = user.Id,
                    SusbcriptionId = user.SubscriptionId
                };

                var serializer = new JavaScriptSerializer();
                var userData = serializer.Serialize(serializeModel);

                var expirationDate = DateTime.Now.Add(FormsAuthentication.Timeout);
                var authTicket = new FormsAuthenticationTicket(1, user.Email, DateTime.Now, expirationDate, persistCookie, userData);

                var encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                Response.Cookies.Add(cookie);

                //We add the custom principal here so it's available right after login.
                var newUser = new WebPrincipal(authTicket.Name)
                {
                    Id = serializeModel.Id,
                    SusbcriptionId = serializeModel.SusbcriptionId
                };

                HttpContext.Current.User = newUser;
            }

            return success;
        }

        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout", Justification = "Login is used more consistently in ASP.Net")]
        public static void Logout()
        {
            FormsAuthentication.SignOut();
        }

        public static bool ChangePassword(string userName, string currentPassword, string newPassword)
        {
            bool success = false;
            try
            {
                var currentUser = System.Web.Security.Membership.GetUser(userName, true /* userIsOnline */);
                success = currentUser.ChangePassword(currentPassword, newPassword);
            }
            catch (ArgumentException)
            {
                // An argument exception is thrown if the new password does not meet the provider's requirements
            }

            return success;
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "This is a helper class, and we are not removing optional parameters from methods in helper classes")]
        public static string CreateAccount(string userName, string password, bool requireConfirmationToken = false)
        {
            throw new NotImplementedException();
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "This is a helper class, and we are not removing optional parameters from methods in helper classes")]
        public static string CreateUserAndAccount(string userName, string password, object propertyValues = null, bool requireConfirmationToken = false)
        {
            throw new NotImplementedException();
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "This is a helper class, and we are not removing optional parameters from methods in helper classes")]
        public static string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow = 1440)
        {
            throw new NotImplementedException();
        }

        public static bool UserExists(string userName)
        {
            VerifyProvider();
            return System.Web.Security.Membership.GetUser(userName) != null;
        }

        public static int GetUserIdFromPasswordResetToken(string token)
        {
            throw new NotImplementedException();
        }

        public static bool IsCurrentUser(long userId)
        {
            VerifyProvider();
            return CurrentUserId == userId;
        }

        public static bool IsCurrentUser(string email)
        {
            VerifyProvider();
            return String.Equals(CurrentUserEmail, email, StringComparison.OrdinalIgnoreCase);
        }

        // Make sure the logged on user is same as the one specified by the id
        private static bool IsUserLoggedOn(long userId)
        {
            VerifyProvider();
            return CurrentUserId == userId;
        }

        // Make sure the user was authenticated
        public static void RequireAuthenticatedUser()
        {
            throw new NotImplementedException();
        }

        // Make sure the user was authenticated
        public static void RequireUser(int userId)
        {
            throw new NotImplementedException();
        }

        public static void RequireUser(string userName)
        {
            throw new NotImplementedException();
        }

        public static void RequireRoles(params string[] roles)
        {
            throw new NotImplementedException();
        }

        private static WebMembershipProvider VerifyProvider()
        {
            var provider = System.Web.Security.Membership.Provider as WebMembershipProvider;
            if (provider == null)
            {
                throw new InvalidOperationException("WebMembership provider is not initialized.");
            }

            return provider;
        }

        public static WebRoleProvider AccessControl
        {
            get
            {
                var provider = System.Web.Security.Roles.Provider as WebRoleProvider;
                if (provider == null)
                {
                    throw new InvalidOperationException("WebRoleProvider is not initialized.");
                }

                return provider;
            }
        }

        public static WebMembershipProvider Membership
        {
            get
            {
                var provider = System.Web.Security.Membership.Provider as WebMembershipProvider;
                if (provider == null)
                {
                    throw new InvalidOperationException("WebMembershipProvider is not initialized.");
                }
                return provider;
            }
        }

        public static bool TryGetCurrentUser(out User user)
        {
            user = null;
            try
            {
                using (var context = new DatabaseContext())
                {
                    user = context.Users.GetById(CurrentUserId);
                    return user != null;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
