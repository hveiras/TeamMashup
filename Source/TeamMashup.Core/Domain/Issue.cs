using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using TeamMashup.Core.Enums;

namespace TeamMashup.Core.Domain
{
    public class Issue : Entity
    {
        public long ProjectId { get; set; }

        public virtual Project Project { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public int TypeValue { get; set; }

        public int StoryPoints { get; set; }

        public virtual ICollection<File> Attachments { get; set; }

        public long ReporterId { get; set; }

        public virtual User Reporter { get; set; }

        public long? AssigneeId { get; set; }

        public virtual User Assignee { get; set; }

        public int StateValue { get; set; }

        public ICollection<string> Tags { get; set; }

        public long? ReleaseId { get; set; }

        public long? IterationId { get; set; }

        public int Priority { get; set; }

        public float TimeSpent { get; set; }

        public float? RemainingEstimate { get; set; }

        public IssueType Type
        {
            get { return (IssueType)TypeValue; }
            set { TypeValue = (int)value; }
        }

        public ScheduleState State
        {
            get { return (ScheduleState)StateValue; }
            set { StateValue = (int)value; }
        }

        protected override bool TryDeleteCascade(out string errorKey)
        {
            errorKey = string.Empty;
            return true;
        }

        public Issue()
        {
            Attachments = new List<File>();
            Tags = new List<string>();
            State = ScheduleState.Backlog;
        }

        public Issue(string summary, IssueType type, long projectId, long reporterId) : this()
        {
            Summary = summary;
            Type = type;
            ProjectId = projectId;
            ReporterId = reporterId;
        }
    }

    public class IssueConfiguration : EntityTypeConfiguration<Issue>
    {
        public IssueConfiguration()
        {
            Ignore(x => x.Type);
            Ignore(x => x.State);

            Property(x => x.Summary)
                .HasMaxLength(255)
                .IsRequired();

            Property(x => x.TypeValue)
                .HasColumnName("Type")
                .IsRequired();

            Property(x => x.StateValue)
                .HasColumnName("State")
                .IsRequired();
        }
    }
}