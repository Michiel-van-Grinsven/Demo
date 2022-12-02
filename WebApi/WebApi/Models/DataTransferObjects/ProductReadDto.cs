using System.Text.Json.Serialization;

namespace WebApi.Models.DataModels
{
    public class ProductReadDto
    {
        [JsonConstructor]
        public ProductReadDto() { }

        public ProductReadDto(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            CreatorId = product.CreatorId;
            CreatedDate = product.CreatedDate;
            UpdatedDate = product.UpdatedDate;
            WeightInGrams = product.WeightInGrams;
            CarbonOutputPerGram = product.CarbonOutputPerGram;
            TotalCarbonOutput = product.TotalCarbonOutput;
            AssignedProjects = product.AssignedProjects.Select(project => project.Id).ToList();
        }

        public Guid Id { get; set; } = Guid.Empty;

        public string Name { get; set; } = string.Empty;

        public Guid CreatorId { get; set; } = Guid.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        public decimal WeightInGrams { get; set; } = 0;

        public decimal CarbonOutputPerGram { get; set; } = 0;

        public decimal TotalCarbonOutput { get; set; } = 0;

        public List<Guid> AssignedProjects { get; set; } = new List<Guid>();
    }
}
