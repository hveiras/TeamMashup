using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using TeamMashup.Core.Enums;

namespace TeamMashup.Core.Domain
{
    public class Release : Entity, IScheduleableEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public long ProjectId { get; set; }

        public int StateValue { get; set; }

        public virtual ICollection<Iteration> Iterations { get; set; }

        public ReleaseState State
        {
            get { return (ReleaseState)StateValue; }
            set { StateValue = (int)value; }
        }

        public virtual ICollection<Iteration> IterationsNonDeleted
        {
            get { return Iterations.NonDeleted().ToList(); }
        }

        protected override bool TryDeleteCascade(out string errorKey)
        {
            errorKey = string.Empty;

            if (State != ReleaseState.Planning)
            {
                errorKey = "OnlyReleasesInPlanningCanBeDeleted";
                return false;
            }

            return true;
        }

        public Release()
        {
            State = ReleaseState.Planning;
        }

        public Release(string name, DateTime from, DateTime to, long projectId) : this()
        {
            Name = name;
            From = from;
            To = to;
            ProjectId = projectId;
        }
    }

    public class ReleaseConfiguration : EntityTypeConfiguration<Release>
    {
        public ReleaseConfiguration()
        {
            Ignore(x => x.State);

            Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            Property(x => x.StateValue)
                .HasColumnName("State")
                .IsRequired();

            Ignore(x => x.IterationsNonDeleted);
        }
    }
}