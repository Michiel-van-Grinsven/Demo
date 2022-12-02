using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using WebApi.Data.Validators;

namespace WebApi.Models.DataModels
{
    public class Product : BaseEntity
    {
        public Product() { }

        public Product(ProductCreateDto dto)
        {
            Name = dto.Name;
            CreatorId = dto.CreatorId;
            WeightInGrams = decimal.Parse(dto.WeightInGrams, CultureInfo.InvariantCulture);
            CarbonOutputPerGram = decimal.Parse(dto.CarbonOutputPerGram, CultureInfo.InvariantCulture);
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
        [RequiredGreaterThanZero(ErrorMessage = "Must be greater than 0.")]
        public decimal WeightInGrams { get; set; }

        [Required]
        [RequiredGreaterThanZero(ErrorMessage = "Must be greater than 0.")]
        public decimal CarbonOutputPerGram { get; set; }

        [NotMapped]
        public decimal TotalCarbonOutput { get { return CarbonOutputPerGram * WeightInGrams; } }

        public ICollection<Project> AssignedProjects { get; set; } = new List<Project>();
    }
}
