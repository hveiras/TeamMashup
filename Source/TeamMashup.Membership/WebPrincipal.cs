using System;
using System.Security.Principal;

namespace TeamMashup.Membership
{
    public class WebPrincipal : IPrincipal
    {
        public IIdentity Identity { get; private set; }

        public bool IsInRole(string role)
        {
            throw new NotSupportedException();
        }

        public long Id { get; set; }

        public long SusbcriptionId { get; set; }

        public string Email { get; set; }

        public WebPrincipal(string email)
        {
            this.Identity = new GenericIdentity(email);
            this.Email = email;
        }
    }
}
