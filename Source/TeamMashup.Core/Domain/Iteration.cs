using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using TeamMashup.Core.Enums;

namespace TeamMashup.Core.Domain
{
    public class Iteration : Entity, IScheduleableEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public long ProjectId { get; set; }

        public long ReleaseId { get; set; }

        public virtual Release Release { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int StateValue { get; set; }

        public virtual ICollection<Issue> UserStories { get; set; }

        public int StoryPointValueInHours { get; set; }

        public IterationState State
        {
            get { return (IterationState)StateValue; }
            set { StateValue = (int)value; }
        }

        public virtual ICollection<Issue> UserStoriesNonDeleted
        {
            get { return UserStories.NonDeleted<Issue>().ToList(); }
        }

        public Iteration()
        {
            State = IterationState.Planning;
        }

        public Iteration(string name, DateTime from, DateTime to, long projectId, long releaseId) : this()
        {
            Name = name;
            From = from;
            To = to;
            ProjectId = projectId;
            ReleaseId = releaseId;
        }

        protected override bool TryDeleteCascade(out string errorKey)
        {
            errorKey = string.Empty;

            if (State != IterationState.Planning)
            {
                errorKey = "OnlyIterationsInPlanningCanBeDeleted";
                return false;
            }

            return true;
        }
    }

    public class IterationConfiguration : EntityTypeConfiguration<Iteration>
    {
        public IterationConfiguration()
        {
            Ignore(x => x.State);
            Ignore(x => x.UserStoriesNonDeleted);

            Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            Property(x => x.StateValue)
                .HasColumnName("Sate")
                .IsRequired();
        }
    }
}