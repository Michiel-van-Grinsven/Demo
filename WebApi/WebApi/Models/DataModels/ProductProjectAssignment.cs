using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebApi.Models.DataModels
{
    [PrimaryKey(nameof(ProductId), nameof(ProjectId))]
    public class ProductProjectAssignment
    {
        [ForeignKey(nameof(DataModels.Product.Id))]
        public int ProductId { get; set; }

        public virtual Product? Product { get; set; }

        [ForeignKey(nameof(DataModels.Project.Id))]
        public int ProjectId { get; set; }

        public virtual Project? Project { get; set; }
    }
}
