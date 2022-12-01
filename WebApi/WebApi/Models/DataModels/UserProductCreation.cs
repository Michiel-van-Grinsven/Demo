using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Areas.Identity.Data;

namespace WebApi.Models.DataModels
{
    [PrimaryKey(nameof(CreatorId), nameof(ProductId))]
    public class UserProductCreation
    {
        [ForeignKey(nameof(WebApiUser.Id))]
        public Guid CreatorId { get; set; } = Guid.Empty;

        public virtual WebApiUser? Creator { get; set; }

        [ForeignKey(nameof(DataModels.Product.Id))]
        public Guid ProductId { get; set; } = Guid.Empty;

        public virtual Product? Product { get; set; }
    }
}
