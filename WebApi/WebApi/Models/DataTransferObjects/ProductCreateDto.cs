using System.ComponentModel.DataAnnotations;
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
            WeightInGrams = product.WeightInGrams.ToString();
            CarbonOutputPerGram = product.CarbonOutputPerGram.ToString();
            AssignedProjects = product.AssignedProjects.Select(project => project.Id).ToList();
        }

        public string Name { get; set; } = string.Empty;

        public string Unit { get; set; } = "g"; 

        public Guid CreatorId { get; set; } = Guid.Empty;

        [Display(Name = "WeightInUnits")]
        public string WeightInGrams { get; set; } = "0";

        public string CarbonOutputPerGram { get; set; } = "0";

        public List<Guid> AssignedProjects { get; set; } = new List<Guid>();
    }
}
