using System;

namespace TeamMashup.Core.Security
{
    public class SecureToken
    {
        public Guid Code { get; set; }

        public DateTime Expires { get; set; }
    }
}