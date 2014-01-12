using System.Collections.Generic;
using TeamMashup.Models.Admin;

namespace TeamMashup.Models.Private
{
    public class HomeModel
    {
        public ICollection<ProjectModel> YourProjects { get; set; }

        public HomeModel()
        {
            YourProjects = new List<ProjectModel>();
        }
    }

    public class SurveyVoteModel
    {
        public long SurveyId { get; set; }

        public long OptionId { get; set; }
    }
}