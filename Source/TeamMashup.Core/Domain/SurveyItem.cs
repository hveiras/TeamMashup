
namespace TeamMashup.Core.Domain
{
    public class SurveyItem : IEntitySet
    {
        public long Id { get; set; }

        public long SurveyId { get; set; }

        public string Description { get; set; }

        public int Votes { get; set; }

        public SurveyItem()
        {
        }

        public SurveyItem(long surveyId, string description) : this()
        {
            SurveyId = surveyId;
            Description = description;
        }
    }
}
