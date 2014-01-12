using System.Data.Entity.ModelConfiguration;

namespace TeamMashup.Core.Domain
{
    public class BillItem : IEntitySet
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }

    public class BillItemConfiguration : EntityTypeConfiguration<BillItem>
    {
        public BillItemConfiguration()
        {
            Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(255);
        }
    }
}