using System;
using System.Data.Entity.ModelConfiguration;

namespace TeamMashup.Core.Domain
{
    //TODO: maybe we should change and make the Code property primary key to improve performance when a code is read externally.
    //See: Password Recovery use case.
    public class PasswordRecovery : ISubscriptionQueryable, IEntitySet
    {
        public long Id { get; set; }

        public Guid Code { get; set; }

        public long SubscriptionId { get; set; }

        public Subscription Subscription { get; set; }

        public DateTime Expires { get; set; }

        public bool Claimed { get; set; }

        public bool IsExpiredOrClaimed()
        {
            return Expires < DateTime.UtcNow || Claimed;
        }
    }

    public class PasswordRecoveryConfiguration : EntityTypeConfiguration<PasswordRecovery>
    {
        public PasswordRecoveryConfiguration()
        {
        }
    }
}