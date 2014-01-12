using System.Collections.Generic;

namespace TeamMashup.Core.Domain
{
    public class Survey : Entity
    {
        public string Title { get; set; }

        public virtual ICollection<SurveyItem> Options { get; set; }

        public bool Active { get; set; }

        public Survey()
        {
        }

        public Survey(string title) : this()
        {
            Title = title;
        }
    }
}
