using Microsoft.AspNetCore.Identity;
using WebApi.Models.DataModels.Interfaces;

namespace WebApi.Areas.Identity.Data
{
    public class WebApiRole : IdentityRole<Guid>, IBaseEntity
    {

    }
}
