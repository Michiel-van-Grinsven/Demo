using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Models.DataModels;
using WebApi.Models.DataModels.Interfaces;

namespace WebApi.Areas.Identity.Data;

public class WebApiUser : IdentityUser<Guid>, ITimeEntity
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [NotMapped]
    public string Name { get { return $"{FirstName} {LastName}"; } }

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [Required]
    public DateTime UpdatedDate { get; set; } = DateTime.Now;

    public ICollection<Project> AssignedProjects { get; set; } = new List<Project>();

    public ICollection<Project> CreatedProjects { get; set; } = new List<Project>();

    public ICollection<Product> CreatedProducts { get; set; } = new List<Product>();
}

