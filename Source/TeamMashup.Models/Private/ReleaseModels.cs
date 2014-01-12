using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TeamMashup.Core.Enums;

namespace TeamMashup.Models.Private
{
    public class ReleaseModel
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public ReleaseState State { get; set; }
    }

    public class PlanReleaseModel
    {
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ICollection<PlanIterationModel> Iterations { get; set; }

        public IEnumerable<string> IterationStatuses { get; set; }

        public bool HasActiveIterations { get; set; }

        public PlanReleaseModel()
        {
            Iterations = new List<PlanIterationModel>();
            IterationStatuses = new List<string>();
        }
    }

    public class PlanIterationModel
    {
        public long Id { get; set; }

        public long ReleaseId { get; set; }

        public string Name { get; set; }

        public IterationState State { get; set; }

        public int MaxStoryPoints { get; set; }

        public int StoryPoints { get; set; }

        public bool HasUserStories { get; set; }
    }
}