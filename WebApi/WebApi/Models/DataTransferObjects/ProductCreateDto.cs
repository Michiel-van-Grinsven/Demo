using System.Text.Json.Serialization;

namespace WebApi.Models.DataModels
{
    public class ProductCreateDto
    {
        [JsonConstructor]
        public ProductCreateDto() { }

        public ProductCreateDto(Product product)
        {
            Name = product.Name;
            CreatorId = product.CreatorId;
            WeightInGrams = product.WeightInGrams;
            CarbonOutputPerGram = product.CarbonOutputPerGram;
            AssignedProjects = product.AssignedProjects.Select(project => project.Id).ToList();
        }

        public string Name { get; set; } = string.Empty;

        public string Unit { get; set; } = "g"; 

        public Guid CreatorId { get; set; } = Guid.Empty;

        public double WeightInGrams { get; set; } = 0.0;

        public double CarbonOutputPerGram { get; set; } = 0.0;

        public List<Guid> AssignedProjects { get; set; } = new List<Guid>();
    }
}
