using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Areas.Identity.Data;

namespace WebApi.Models.DataModels
{
    [PrimaryKey(nameof(CreatorId), nameof(ProductId))]
    public class UserProductCreation
    {
        [ForeignKey(nameof(WebApiUser.Id))]
        public string CreatorId { get; set; } = string.Empty;

        public virtual WebApiUser? Creator { get; set; }

        [ForeignKey(nameof(DataModels.Product.Id))]
        public int ProductId { get; set; }

        public virtual Product? Product { get; set; }
    }
}
