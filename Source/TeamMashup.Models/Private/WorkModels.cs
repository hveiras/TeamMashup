using System;

namespace TeamMashup.Models.Private
{
    public class BoardModel
    {
        public long IterationId { get; set; }

        public string IterationName { get; set; }
    }

    public class LogWorkModel
    {
        public long IssueId { get; set; }

        public DateTime? DateStarted { get; set; }

        public float TimeSpent { get; set; }

        public float? RemainingEstimate { get; set; }

        public string WorkDescription { get; set; }
    }
}