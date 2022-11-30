using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Areas.Identity.Data;

namespace WebApi.Models.DataModels
{
    [PrimaryKey(nameof(CreatorId), nameof(ProjectId))]
    public class UserProjectCreation
    {
        [ForeignKey(nameof(WebApiUser.Id))]
        public string CreatorId { get; set; } = string.Empty;

        public virtual WebApiUser? Creator { get; set; }

        [ForeignKey(nameof(DataModels.Project.Id))]
        public int ProjectId { get; set; }

        public virtual Project? Project { get; set; }
    }
}
