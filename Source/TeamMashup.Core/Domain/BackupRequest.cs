using System.Data.Entity.ModelConfiguration;
using TeamMashup.Core.Enums;

namespace TeamMashup.Core.Domain
{
    public class BackupRequest : Entity
    {
        public int TypeValue { get; set; }

        public int StateValue { get; set; }

        public int ScheduleValue { get; set; }

        public string Path { get; set; }

        public BackupType Type
        {
            get { return (BackupType)TypeValue; }
            set { TypeValue = (int)value; }
        }

        public BackupState State
        {
            get { return (BackupState)StateValue; }
            set { StateValue = (int)value; }
        }

        public BackupSchedule Schedule
        {
            get { return (BackupSchedule)ScheduleValue; }
            set { ScheduleValue = (int)value; }
        }

        public BackupRequest()
        {
            State = BackupState.Pending;
        }

        public BackupRequest(BackupType type, BackupSchedule schedule) : this()
        {
            Type = type;
            Schedule = schedule;
        }
    }

    public class BackupRequestConfiguration : EntityTypeConfiguration<BackupRequest>
    {
        public BackupRequestConfiguration()
        {
            Ignore(x => x.Type);
            Ignore(x => x.State);
            Ignore(x => x.Schedule);

            Property(x => x.TypeValue)
                .HasColumnName("Type")
                .IsRequired();

            Property(x => x.StateValue)
                .HasColumnName("State")
                .IsRequired();

            Property(x => x.ScheduleValue)
                .HasColumnName("Schedule")
                .IsRequired();
        }
    }
}
