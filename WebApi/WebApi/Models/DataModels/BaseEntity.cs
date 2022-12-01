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
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [Required]
        [ForeignKey(nameof(WebApiUser.Id))]
        public Guid CreatorId { get; set; } = Guid.Empty;

        public virtual WebApiUser? Creator { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
