using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Areas.Identity.Data;

namespace WebApi.Models.DataModels
{
    public class Project : BaseEntity
    {
        public virtual ICollection<WebApiUser> AssignedUsers { get; set; } = new List<WebApiUser>();

        public virtual ICollection<Product> AssignedProducts { get; set; } = new List<Product>();

        [NotMapped]
        public double TotalCarbonOutput
        {
            get
            {
                var total = 0.0;
                foreach (var product in AssignedProducts)
                {
                    total += product.TotalCarbonOutput;
                }
                return total;
            }
        }

    }
}
