using System.Data.Entity.ModelConfiguration;

namespace TeamMashup.Core.Domain
{
    public class Country : IEntitySet, IUniqueNamedEntity
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public Country() { }

        public Country(string name) : this()
        {
            Name = name;
        }
    }

    public class CountryConfiguration : EntityTypeConfiguration<Country>
    {
        public CountryConfiguration()
        {
            Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}