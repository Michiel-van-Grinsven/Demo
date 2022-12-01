using System.Text.Json.Serialization;

namespace WebApi.Models.DataModels
{
    public class ProjectUpdateDto
    {
        [JsonConstructor]
        public ProjectUpdateDto() { }

        public ProjectUpdateDto(Project project)
        {
            Name = project.Name;
            AssignedUsers = project.AssignedUsers.Select(user => user.Id).ToList();
            AssignedProducts = project.AssignedUsers.Select(user => user.Id).ToList();
        }

        public string Name { get; set; } = string.Empty;

        public List<Guid> AssignedUsers { get; set; } = new List<Guid>();

        public List<Guid> AssignedProducts { get; set; } = new List<Guid>();
    }
}
