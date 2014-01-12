using System.Collections.Generic;

namespace TeamMashup.Models.Admin
{
    public class ProjectModel
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }

    public class ProjectAddModel
    {
        public ProjectAddModel()
        {
            this.SelectedMembers = new List<ProjectAssignmentModel>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public IEnumerable<ProjectAssignmentModel> SelectedMembers { get; set; }
    }

    public class ProjectAssignmentModel
    {
        public long UserId { get; set; }
        public int RoleId { get; set; }
    }

    public class UserModels
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long ProfilePictureId { get; set; }
    }
}