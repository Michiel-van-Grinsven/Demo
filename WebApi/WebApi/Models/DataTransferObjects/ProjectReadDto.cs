using System.Text.Json.Serialization;

namespace WebApi.Models.DataModels
{
    public class ProjectReadDto
    {
        [JsonConstructor]
        public ProjectReadDto() { }

        public ProjectReadDto(Project project)
        {
            Id = project.Id;
            Name = project.Name;
            CreatorId = project.CreatorId;
            CreatedDate = project.CreatedDate;
            UpdatedDate = project.UpdatedDate;
            AssignedUsers = project.AssignedUsers.Select(user => user.Id).ToList();
            AssignedProducts = project.AssignedProducts.Select(product => product.Id).ToList();
        }

        public Guid Id { get; set; } = Guid.Empty;

        public string Name { get; set; } = string.Empty;

        public Guid CreatorId { get; set; } = Guid.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        public List<Guid> AssignedUsers { get; set; } = new List<Guid>();

        public List<Guid> AssignedProducts { get; set; } = new List<Guid>();

    }
}
