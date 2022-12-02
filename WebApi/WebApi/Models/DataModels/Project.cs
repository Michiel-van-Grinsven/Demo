using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Areas.Identity.Data;

namespace WebApi.Models.DataModels
{
    public class Project : BaseEntity
    {
        public Project() { }

        public Project(ProjectCreateDto dto)
        {
            Name = dto.Name;
            CreatorId = dto.CreatorId;
        }

        public Project(ProjectReadDto dto)
        {
            Id = dto.Id;
            Name = dto.Name;
            CreatorId = dto.CreatorId;
            CreatedDate = dto.CreatedDate;
            UpdatedDate = DateTime.Now;
        }

        public Project(ProjectUpdateDto dto)
        {
            Name = dto.Name;
        }

        public ICollection<WebApiUser> AssignedUsers { get; set; } = new List<WebApiUser>();

        public ICollection<Product> AssignedProducts { get; set; } = new List<Product>();

        [NotMapped]
        public decimal TotalCarbonOutput
        {
            get
            {
                decimal total = 0;
                foreach (var product in AssignedProducts)
                {
                    total += product.TotalCarbonOutput;
                }
                return total;
            }
        }

    }
}
