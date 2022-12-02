using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DataModels.Interfaces
{
    public interface IBaseEntity
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
