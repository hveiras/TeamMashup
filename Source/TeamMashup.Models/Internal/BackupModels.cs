using System;
using TeamMashup.Core.Enums;

namespace TeamMashup.Models.Internal
{
    public class BackupRequestModel
    {
        public long Id { get; set; }

        public string Path { get; set; }

        public BackupType Type { get; set; }

        public BackupState State { get; set; }

        public BackupSchedule Schedule { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}