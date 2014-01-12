using System;
using System.Web.Security;

namespace TeamMashup.Membership
{
    public class WebMembershipUser : MembershipUser
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long SubscriptionId { get; set; }

        public WebMembershipUser(string providername, string username, object providerUserKey, string email, string passwordQuestion, string comment,
                                    bool isApproved, bool isLockedOut, DateTime creationDate, DateTime lastLoginDate, DateTime lastActivityDate,
                                    DateTime lastPasswordChangedDate, DateTime lastLockedOutDate, long userId, long subscriptionId, string name) :
                                    base(providername, username, providerUserKey, email, passwordQuestion, comment, isApproved, isLockedOut,
                                         creationDate, lastLoginDate, lastPasswordChangedDate, lastActivityDate, lastLockedOutDate)
        {
            Id = userId;
            SubscriptionId = subscriptionId;
            Name = name;
        }
    }
}
