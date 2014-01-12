using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;

namespace TeamMashup.Core.Domain
{
    public class Project : Entity, ISubscriptionQueryable, IUniqueNamedEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public long SubscriptionId { get; set; }

        public bool IsEnded { get; set; }

        public DateTime? EndedDate { get; set; }

        public virtual ICollection<ProjectAssignment> Assignments { get; set; }

        public ICollection<ProjectAssignment> AssignmentsNonDeleted
        {
            get { return Assignments.NonDeleted().ToList(); }
        }

        public Project() { }

        public Project(long subscriptionId, string name) : this()
        {
            SubscriptionId = subscriptionId;
            Name = name;
        }

        public void End()
        {
            IsEnded = true;
            EndedDate = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsEnded = false;
            EndedDate = null;
        }
    }

    public class ProjectConfiguration : EntityTypeConfiguration<Project>
    {
        public ProjectConfiguration()
        {
            Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            Ignore(x => x.AssignmentsNonDeleted);
        }
    }
}