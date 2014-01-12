using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TeamMashup.Core.Enums;

namespace TeamMashup.Models.Private
{
    public class IterationModel
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public IterationState State { get; set; }

        public long ReleaseId { get; set; }
    }

    public class IterationResourcesModel
    {
        public long IterationId { get; set; }

        public IEnumerable<IterationResourceModel> Resources { get; set; }

        public int StoryPointValue { get; set; }

        public IterationResourcesModel()
        {
            Resources = new List<IterationResourceModel>();
        }
    }

    public class IterationResourceModel
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public long PictureId { get; set; }

        public string UserName { get; set; }

        public int Capacity { get; set; }

        public int Velocity { get; set; }
    }
}