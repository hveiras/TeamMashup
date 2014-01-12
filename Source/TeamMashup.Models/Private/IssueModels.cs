using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TeamMashup.Core.Enums;

namespace TeamMashup.Models.Private
{
    public class IssueModel
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Summary { get; set; }

        public string Description { get; set; }

        public IEnumerable<long> AttachmentIds { get; set; }

        public long ReporterId { get; set; }

        public string ReporterName { get; set; }

        public long? AssigneeId { get; set; }

        public string AssigneeName { get; set; }

        public IssueType Type { get; set; }

        public ScheduleState State { get; set; }

        public int StoryPoints { get; set; }

        public int Priority { get; set; }

        public float TimeSpent { get; set; }

        public string RemainingEstimate { get; set; }

        public ICollection<string> Tags { get; set; }

        public IssueModel()
        {
            AttachmentIds = new List<long>();
            Tags = new List<string>();
        }
    }


    public class AddIssueInlineModel
    {
        [Required]
        [MaxLength(255)]
        public string Summary { get; set; }

        [Required]
        public IssueType Type { get; set; }
    }
}