using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Areas.Identity.Data;

namespace WebApi.Models.DataModels
{
    [PrimaryKey(nameof(UserId), nameof(ProjectId))]
    public class UserProjectAssignment
    {
        [ForeignKey(nameof(WebApiUser.Id))]
        public Guid UserId { get; set; } = Guid.Empty;

        public virtual WebApiUser? User { get; set; }

        [ForeignKey(nameof(DataModels.Project.Id))]
        public Guid ProjectId { get; set; } = Guid.Empty;

        public virtual Project? Project { get; set; }
    }
}
