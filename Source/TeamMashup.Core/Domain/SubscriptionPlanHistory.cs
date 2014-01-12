using System;
using System.Data.Entity.ModelConfiguration;

namespace TeamMashup.Core.Domain
{
    public class SubscriptionPlanHistory : ISubscriptionQueryable, IEntitySet
    {
        public long Id { get; set; }

        public long SubscriptionId { get; set; }

        public long SubscriptionPlanId { get; set; }

        public DateTime From { get; set; }

        public DateTime? To { get; set; }
    }

    public class SubscriptionPlanHistoryConfiguration : EntityTypeConfiguration<SubscriptionPlanHistory>
    {
        public SubscriptionPlanHistoryConfiguration()
        {
        }
    }
}