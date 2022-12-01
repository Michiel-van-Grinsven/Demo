using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebApi.Models.DataModels
{
    [PrimaryKey(nameof(ProductId), nameof(ProjectId))]
    public class ProductProjectAssignment
    {
        [ForeignKey(nameof(DataModels.Product.Id))]
        public Guid ProductId { get; set; } = Guid.Empty;

        public virtual Product? Product { get; set; }

        [ForeignKey(nameof(DataModels.Project.Id))]
        public Guid ProjectId { get; set; } = Guid.Empty;

        public virtual Project? Project { get; set; }
    }
}
