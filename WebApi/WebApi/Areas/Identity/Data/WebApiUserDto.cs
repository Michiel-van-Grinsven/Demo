using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Areas.Identity.Data;

[NotMapped]
public class WebApiUserDto
{
    public string CreatorId { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
}

