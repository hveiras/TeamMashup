using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using TeamMashup.Core.Enums;

namespace TeamMashup.Core.Domain
{
    public class Claim : IEntitySet, IUniqueNamedEntity
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Role> Groups { get; set; }

        public int ScopeValue { get; internal set; }

        public AssetScope Scope
        {
            get { return (AssetScope)ScopeValue; }
            set { ScopeValue = (int)value; }
        }

        public bool IsSystemClaim { get; set; }
    }

    public class ClaimConfiguration : EntityTypeConfiguration<Claim>
    {
        public ClaimConfiguration()
        {
            Ignore(x => x.Scope);

            //TODO: Create unique constraint over Name.
            Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}