using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace TeamMashup.Core.Domain
{
    public class ProjectAssignment : Entity
    {
        public bool Active { get; set; }

        public long ProjectId { get; set; }

        public long UserId { get; set; }

        public virtual ICollection<IterationResource> IterationResources { get; set; }

        public long RoleId { get; set; }

        public virtual ProjectAssignmentRole Role { get; set; }
    }

    public class ProjectAssignmentConfiguration : EntityTypeConfiguration<ProjectAssignment>
    {
        public ProjectAssignmentConfiguration()
        {
        }
    }
}