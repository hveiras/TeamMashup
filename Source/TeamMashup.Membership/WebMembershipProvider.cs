using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Configuration;
using System.Web.Security;
using TeamMashup.Core;
using TeamMashup.Core.Domain;
using TeamMashup.Core.Security;

namespace TeamMashup.Membership
{
    public class WebMembershipProvider : MembershipProvider
    {
        private string applicationName;
        private int newPasswordLength = 8;
        private string connectionString;
        private bool enablePasswordReset;
        private bool enablePasswordRetrieval;
        private bool requiresQuestionAndAnswer;
        private bool requiresUniqueEmail;
        private int maxInvalidPasswordAttempts;
        private int passwordAttemptWindow;
        private MembershipPasswordFormat passwordFormat;
        private int minRequiredNonAlphanumericCharacters;
        private int minRequiredPasswordLength;
        private string passwordStrengthRegularExpression;

        #region Properties

        public override string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        public override bool EnablePasswordReset
        {
            get
            {
                return enablePasswordReset;
            }
        }

        public override bool EnablePasswordRetrieval
        {
            get
            {
                return enablePasswordRetrieval;
            }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                return requiresQuestionAndAnswer;
            }
        }

        public override bool RequiresUniqueEmail
        {
            get
            {
                return requiresUniqueEmail;
            }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                return maxInvalidPasswordAttempts;
            }
        }

        public override int PasswordAttemptWindow
        {
            get
            {
                return passwordAttemptWindow;
            }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get
            {
                return passwordFormat;
            }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get
            {
                return minRequiredNonAlphanumericCharacters;
            }
        }

        public override int MinRequiredPasswordLength
        {
            get
            {
                return minRequiredPasswordLength;
            }
        }

        public override string PasswordStrengthRegularExpression
        {
            get
            {
                return passwordStrengthRegularExpression;
            }
        }

        #endregion

        #region MembershipProvider Overrides

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                string configPath = "~/web.config";
                Configuration NexConfig = WebConfigurationManager.OpenWebConfiguration(configPath);
                MembershipSection section = (MembershipSection)NexConfig.GetSection("system.web/membership");
                ProviderSettingsCollection settings = section.Providers;
                NameValueCollection membershipParams = settings[section.DefaultProvider].Parameters;
                config = membershipParams;
            }

            if (name == null || name.Length == 0)
            {
                name = "WebMembershipProvider";
            }

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Custom Membership Provider");
            }

            //Initialize the abstract base class.
            base.Initialize(name, config);

            applicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            maxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            passwordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            minRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredAlphaNumericCharacters"], "1"));
            minRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            passwordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], String.Empty));
            enablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            enablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            requiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            requiresUniqueEmail = true;

            ConnectionStringSettings ConnectionStringSettings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];

            if ((ConnectionStringSettings == null) || (ConnectionStringSettings.ConnectionString.Trim() == String.Empty))
            {
                throw new ProviderException("Connection string cannot be blank.");
            }

            connectionString = ConnectionStringSettings.ConnectionString;
        }

        public override bool ChangePassword(string email, string oldPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword)) return false;

            if (oldPassword == newPassword) return false;

            try
            {
                using (var context = new DatabaseContext())
                {
                    var user = (from u in context.Users
                                where u.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase) && !u.Deleted
                                select u).FirstOrDefault();

                    if (user == null)
                        return false;

                    if (string.IsNullOrWhiteSpace(user.Password))
                        return false;

                    user.Password = HashPassword(newPassword);

                    context.SaveChanges();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public WebMembershipUser CreateUser(long subscriptionId, string name, string email, string password, out MembershipCreateStatus status, long languageId = Constants.InvalidId)
        {
            var args = new ValidatePasswordEventArgs(email, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && IsEmailInUse(email))
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            var membershipUser = GetUser(email);

            if (membershipUser == null)
            {
                try
                {
                    using (var context = new DatabaseContext())
                    {
                        Language language;
                        if(languageId == Constants.InvalidId)
                            language = context.Languages.GetByName("English");
                        else
                            language = context.Languages.GetById(languageId);

                        if(language == null)
                        {
                            language = new Language("en", "English");
                            context.Languages.Add(language);
                            context.SaveChanges();
                        }

                        var user = new User(name, email, SecurityManager.Hash(password), subscriptionId);

                        context.Users.Add(user);
                        context.SaveChanges();

                        var profile = new UserProfile(user.Id, language.Id);
                        context.UserProfiles.Add(profile);
                        context.SaveChanges();

                        status = MembershipCreateStatus.Success;

                        return GetUser(email);
                    }
                }
                catch
                {
                    status = MembershipCreateStatus.ProviderError;
                }
            }
            else
            {
                status = MembershipCreateStatus.DuplicateEmail;
            }

            return null;
        }

        public WebMembershipUser GetUser(string email)
        {
            WebMembershipUser membershipUser = null;
            using (var context = new DatabaseContext())
            {
                var user = (from u in context.Users
                            where u.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase) && !u.Deleted
                            select u).FirstOrDefault();

                if (user != null)
                {
                    membershipUser = new WebMembershipUser(this.Name, user.Email, null, user.Email, string.Empty, string.Empty, true,
                                                        false, user.CreatedDate, DateTime.UtcNow, DateTime.UtcNow, default(DateTime),
                                                        default(DateTime), user.Id, user.SubscriptionId, user.Name);

                }
            }

            return membershipUser;
        }

        public override bool ValidateUser(string email, string password)
        {
            using (var context = new DatabaseContext())
            {
                var user = (from u in context.Users
                            where u.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase) && !u.Deleted
                            select u).FirstOrDefault();

                if (user == null)
                    return false;

                var hashedPassword = HashPassword(password);

                return hashedPassword == user.Password;
            }
        }

        public bool IsEmailInUse(string email)
        {
            using (var context = new DatabaseContext())
            {
                return context.Users.Any(x => x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public override bool DeleteUser(string email, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override string ResetPassword(string email, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string email)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
            {
                return defaultValue;
            }

            return configValue;
        }

        private string HashPassword(string password)
        {
            return SecurityManager.Hash(password);
        }

        #endregion

        #region NotSupported Operations

        [Obsolete("This method is just to mantain compatibility with MembershipProvider base class")]
        public override MembershipUser GetUser(string email, bool userIsOnline)
        {
            throw new NotSupportedException();
        }

        [Obsolete("This method is just to mantain compatibility with MembershipProvider base class")]
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotSupportedException();
        }

        [Obsolete("This method is just to mantain compatibility with MembershipProvider base class")]
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotSupportedException();
        }

        [Obsolete("This method is just to mantain compatibility with MembershipProvider base class")]
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotSupportedException();
        }

        [Obsolete("This method is just to mantain compatibility with MembershipProvider base class")]
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotSupportedException();
        }

        [Obsolete("This method is just to mantain compatibility with MembershipProvider base class")]
        public override int GetNumberOfUsersOnline()
        {
            throw new NotSupportedException();
        }

        [Obsolete("This method is just to mantain compatibility with MembershipProvider base class")]
        public override string GetPassword(string username, string answer)
        {
            throw new NotSupportedException();
        }

        [Obsolete("This method is just to mantain compatibility with MembershipProvider base class")]
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException();
        }

        [Obsolete("This method is just to mantain compatibility with MembershipProvider base class")]
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotSupportedException();
        }

        [Obsolete("This method is just to mantain compatibility with MembershipProvider base class")]
        public override string GetUserNameByEmail(string email)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
