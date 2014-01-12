using System;
using System.Data.Entity.ModelConfiguration;

namespace TeamMashup.Core.Domain
{
    public class UserActivation : Entity
    {
        public Guid Code { get; set; }

        public long UserId { get; set; }

        public virtual User User { get; set; }

        public DateTime Expires { get; set; }

        public bool Claimed { get; set; }

        public bool IsExpiredOrClaimed()
        {
            return Expires < DateTime.UtcNow || Claimed;
        }
    }

    public class UserActivationConfiguration : EntityTypeConfiguration<UserActivation>
    {
        public UserActivationConfiguration()
        {
        }
    }
}