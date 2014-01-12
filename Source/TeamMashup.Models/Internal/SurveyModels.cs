using System.Collections.Generic;

namespace TeamMashup.Models.Internal
{
    public class SurveyModel
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public IEnumerable<SurveyItemModel> Options { get; set; }

        public bool Active { get; set; }

        public SurveyModel()
        {
            Options = new List<SurveyItemModel>();
        }
    }

    public class SurveyItemModel
    {
        public long Id { get; set; }

        public long SurveyId { get; set; }

        public string Description { get; set; }

        public int Votes { get; set; }
    }
}