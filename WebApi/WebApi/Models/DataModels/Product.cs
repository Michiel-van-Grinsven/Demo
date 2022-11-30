using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models.DataModels
{
    public class Product : BaseEntity
    {
        [Required]
        public double WeightInGrams { get; set; }

        [Required]
        public double CarbonOutputPerGram { get; set; }

        [NotMapped]
        public double TotalCarbonOutput { get { return CarbonOutputPerGram * WeightInGrams; } }

        public virtual ICollection<Project> AssignedProjects { get; set; } = new List<Project>();
    }
}
