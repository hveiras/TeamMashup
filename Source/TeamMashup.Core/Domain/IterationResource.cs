using System.Data.Entity.ModelConfiguration;

namespace TeamMashup.Core.Domain
{
    public class IterationResource : IEntitySet
    {
        public long Id { get; set; }

        public long ProjectAssignmentId { get; set; }

        public long IterationId { get; set; }

        public int Capacity { get; set; }

        public int Velocity { get; set; }
    }

    public class IterationResourceConfiguration : EntityTypeConfiguration<IterationResource>
    {
        public IterationResourceConfiguration()
        {
        }
    }
}