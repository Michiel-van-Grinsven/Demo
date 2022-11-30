using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Areas.Identity.Data;
using WebApi.Models.DataModels.Interfaces;

namespace WebApi.Models.DataModels
{
    public abstract class BaseEntity : IBaseEntity, ITimeEntity
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [Required]
        [ForeignKey(nameof(WebApiUser.Id))]
        public string CreatorId { get; set; } = string.Empty;

        public virtual WebApiUser? Creator { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime UpdatedDate { get; set; }
    }
}
