using System;
using System.Data.Entity.ModelConfiguration;
using TeamMashup.Core.Enums;

namespace TeamMashup.Core.Domain
{
    public class Log : Entity, ISubscriptionQueryable
    {
        public long SubscriptionId { get; set; }

        internal int LevelValue { get; set; }

        public DateTime DateTime { get; set; }

        public string Message { get; set; }

        public string Source { get; set; }
        
        public string StackTrace { get; set; }

        public LogEntryLevel Level
        {
            get { return (LogEntryLevel)LevelValue; }
            set { LevelValue = (int)value; }
        }

        public Log()
        {
            this.DateTime = DateTime.UtcNow;
        }
    }

    public class LogConfiguration : EntityTypeConfiguration<Log>
    {
        public LogConfiguration()
        {
            Ignore(x => x.Level);

            Property(x => x.Source)
                .HasMaxLength(255);

            Property(x => x.LevelValue)
                .HasColumnName("Level")
                .IsRequired();
        }
    }
}