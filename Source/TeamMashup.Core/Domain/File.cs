using System;
using System.Data.Entity.ModelConfiguration;

namespace TeamMashup.Core.Domain
{
    public class File : Entity, ISubscriptionQueryable
    {
        public Guid UniqueId { get; set; }

        public long SubscriptionId { get; set; }

        public Subscription Subscription { get; set; }

        public long? UserId { get; set; }

        public User User { get; set; }

        public string FileName { get; set; }

        public byte[] Content { get; set; }

        public string MimeType { get; set; }
    }

    public class FileConfiguration : EntityTypeConfiguration<File>
    {
        public FileConfiguration()
        {
            Property(x => x.UniqueId).IsRequired();

            Property(x => x.FileName)
                .IsRequired()
                .HasMaxLength(255);

            Property(x => x.Content)
                .IsRequired();

            Property(x => x.MimeType)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}