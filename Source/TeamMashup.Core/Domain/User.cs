using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using TeamMashup.Core.Enums;

namespace TeamMashup.Core.Domain
{
    public class User : Entity, ISubscriptionQueryable
    {
        public string Email { get; set; }

        public string Name { get; set; }

        public string FirstName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name))
                    return Name;

                var parts = Name.Split(null);
                return parts[0];
            }
        }

        public long SubscriptionId { get; set; }

        public string Password { get; set; }

        public virtual ICollection<Role> Roles { get; set; }

        public virtual ICollection<ProjectAssignment> Assignments { get; set; }

        public int ScopeValue { get; internal set; }

        public AssetScope Scope
        {
            get { return (AssetScope)ScopeValue; }
            set { ScopeValue = (int)value; }
        }

        public bool Enabled { get; set; }

        public ICollection<ProjectAssignment> AssignmentsNonDeleted
        {
            get { return Assignments.NonDeleted<ProjectAssignment>().ToList(); }
        }

        public User() 
        {
            Enabled = true;
            SubscriptionId = Constants.InvalidId;
        }

        public User(string name, string email, string password) : this()
        {
            Name = name;
            Email = email;
            Password = password;
        }

        public User(string name, string email, string password, long subscriptionId) : this(name, email, password)
        {
            SubscriptionId = subscriptionId;
        }
    }

    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            Ignore(x => x.Scope);

            Ignore(x => x.FirstName);

            Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);

            Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            Property(x => x.Password)
                .IsRequired()
                .HasMaxLength(100);

            HasMany(u => u.Roles)
                    .WithMany(g => g.Users)
                    .Map(map =>
                    {
                        map.MapLeftKey("RoleId");
                        map.MapRightKey("UserId");
                        map.ToTable("RoleUser");
                    });

            Ignore(x => x.AssignmentsNonDeleted);
        }
    }
}