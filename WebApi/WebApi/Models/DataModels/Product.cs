using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models.DataModels
{
    public class Product : BaseEntity
    {
        public Product() { }

        public Product(ProductCreateDto dto)
        {
            Name = dto.Name;
            CreatorId = dto.CreatorId;
            WeightInGrams = dto.WeightInGrams;
            CarbonOutputPerGram = dto.CarbonOutputPerGram;
        }

        public Product(ProductReadDto dto)
        {
            Id = dto.Id;
            Name = dto.Name;
            CreatorId = dto.CreatorId;
            CreatedDate = dto.CreatedDate;
            UpdatedDate = DateTime.Now;
            WeightInGrams = dto.WeightInGrams;
            CarbonOutputPerGram = dto.CarbonOutputPerGram;
        }

        public Product(ProductUpdateDto dto)
        {
            Name = dto.Name;
            WeightInGrams = dto.WeightInGrams;
            CarbonOutputPerGram = dto.CarbonOutputPerGram;
        }

        [Required]
        public double WeightInGrams { get; set; }

        [Required]
        public double CarbonOutputPerGram { get; set; }

        [NotMapped]
        public double TotalCarbonOutput { get { return CarbonOutputPerGram * WeightInGrams; } }

        public ICollection<Project> AssignedProjects { get; set; } = new List<Project>();
    }
}
