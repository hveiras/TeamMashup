using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace TeamMashup.Core.Domain
{
    public class SubscriptionPlan : Entity, IUniqueNamedEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<string> Features { get; set; }

        public int MaxUsers { get; set; }

        public int MaxStorage { get; set; }

        public decimal Price { get; set; }

        public SubscriptionPlan() { }

        public SubscriptionPlan(string name, decimal price, int maxUsers) : this()
        {
            Name = name;
            Price = price;
            MaxUsers = maxUsers;
        }
    }

    public class SubscriptionPlanConfiguration : EntityTypeConfiguration<SubscriptionPlan>
    {
        public SubscriptionPlanConfiguration()
        {
            Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}