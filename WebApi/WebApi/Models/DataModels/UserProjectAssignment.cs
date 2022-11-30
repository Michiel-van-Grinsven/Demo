using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Areas.Identity.Data;

namespace WebApi.Models.DataModels
{
    [PrimaryKey(nameof(UserId), nameof(ProjectId))]
    public class UserProjectAssignment
    {
        [ForeignKey(nameof(WebApiUser.Id))]
        public string UserId { get; set; } = string.Empty;

        public virtual WebApiUser? User { get; set; }

        [ForeignKey(nameof(DataModels.Project.Id))]
        public int ProjectId { get; set; }

        public virtual Project? Project { get; set; }
    }
}
