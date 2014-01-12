
namespace TeamMashup.Core.Domain
{
    public class ProjectAssignmentRole : IEntitySet
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public ProjectAssignmentRole() { }

        public ProjectAssignmentRole(string name) : this()
        {
            Name = name;
        }
    }
}
