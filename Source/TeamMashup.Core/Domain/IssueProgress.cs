using System;
using System.Data.Entity.ModelConfiguration;
using TeamMashup.Core.Enums;

namespace TeamMashup.Core.Domain
{
    public class IssueProgress
    {
        public long Id { get; set; }

        public long ProjectId { get; set; }

        public long IssueId { get; set; }

        public int StoryPoints { get; set; }

        public long? AssigneeId { get; set; }

        public int StateValue { get; set; }

        public long? ReleaseId { get; set; }

        public long? IterationId { get; set; }

        public int TypeValue { get; set; }

        public DateTime Date { get; set; }

        public ScheduleState State
        {
            get { return (ScheduleState)StateValue; }
            set { StateValue = (int)value; }
        }

        public IssueProgressType Type
        {
            get { return (IssueProgressType)TypeValue; }
            set { TypeValue = (int)value; }
        }
    }

    public class IssueProgressConfiguration : EntityTypeConfiguration<IssueProgress>
    {
        public IssueProgressConfiguration()
        {
            Ignore(x => x.State);

            Property(x => x.StateValue)
                .HasColumnName("State")
                .IsRequired();
        }
    }
}
