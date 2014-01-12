using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace TeamMashup.Core.Domain
{
    public class Bill : ISubscriptionQueryable, IEntitySet
    {
        public long Id { get; set; }

        public long SubscriptionId { get; set; }

        public Subscription Subscription { get; set; }

        public DateTime Date { get; set; }

        public string TributaryId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerAddress { get; set; }

        public int? CustomerCountryId { get; set; }

        public virtual Country CustomerCountry { get; set; }

        public string ExportType { get; set; }

        public virtual ICollection<BillItem> Items { get; set; }
    }

    public class BillConfiguration : EntityTypeConfiguration<Bill>
    {
        public BillConfiguration()
        {
            Property(x => x.TributaryId)
                .IsRequired()
                .HasMaxLength(50);

            Property(x => x.CustomerName)
                .IsRequired()
                .HasMaxLength(100);

            Property(x => x.CustomerAddress)
                .IsRequired()
                .HasMaxLength(100);

            Property(x => x.ExportType)
                .HasMaxLength(50);

            HasMany(x => x.Items)
                .WithRequired();
        }
    }
}