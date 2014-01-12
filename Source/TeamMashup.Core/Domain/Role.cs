using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using TeamMashup.Core.Enums;

namespace TeamMashup.Core.Domain
{
    public class Role : IEntitySet, IUniqueNamedEntity
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Claim> Claims { get; set; }

        public virtual ICollection<User> Users { get; set; }

        public int ScopeValue { get; internal set; }

        public AssetScope Scope
        {
            get { return (AssetScope)ScopeValue; }
            set { ScopeValue = (int)value; }
        }

        public bool IsSystemRole { get; set; }

        public Role() { }

        public Role(string name) : this()
        {
            this.Name = name;
        }
    }

    public class RoleConfiguration : EntityTypeConfiguration<Role>
    {
        public RoleConfiguration()
        {
            Ignore(x => x.Scope);

            Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            HasMany(g => g.Claims)
           .WithMany(c => c.Groups)
           .Map(map =>
           {
               map.MapLeftKey("ClaimId");
               map.MapRightKey("RoleId");
               map.ToTable("RoleClaim");
           });

            HasMany(g => g.Users)
                .WithMany(u => u.Roles)
                .Map(map =>
                {
                    map.MapLeftKey("UserId");
                    map.MapRightKey("RoleId");
                    map.ToTable("RoleUser");
                });
        }
    }
}