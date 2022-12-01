using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Areas.Identity.Data;

namespace WebApi.Models.DataModels
{
    [PrimaryKey(nameof(CreatorId), nameof(ProjectId))]
    public class UserProjectCreation
    {
        [ForeignKey(nameof(WebApiUser.Id))]
        public Guid CreatorId { get; set; } = Guid.Empty;

        public virtual WebApiUser? Creator { get; set; }

        [ForeignKey(nameof(DataModels.Project.Id))]
        public Guid ProjectId { get; set; } = Guid.Empty;

        public virtual Project? Project { get; set; }
    }
}
