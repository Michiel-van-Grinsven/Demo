using System.Text.Json.Serialization;

namespace WebApi.Models.DataModels
{
    public class ProjectCreateDto
    {
        [JsonConstructor]
        public ProjectCreateDto() { }

        public ProjectCreateDto(Project project)
        {
            Name = project.Name;
            CreatorId = project.CreatorId;
            AssignedUsers = project.AssignedUsers.Select(user => user.Id).ToList();
            AssignedProducts = project.AssignedUsers.Select(user => user.Id).ToList();
        }

        public string Name { get; set; } = string.Empty;

        public Guid CreatorId { get; set; } = Guid.Empty;

        public List<Guid> AssignedUsers { get; set; } = new List<Guid>();

        public List<Guid> AssignedProducts { get; set; } = new List<Guid>();
    }
}
