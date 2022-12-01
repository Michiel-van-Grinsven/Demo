using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models.DataModels;

namespace WebApi.Controllers.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly WebApiContext _context;

        public ProjectsController(WebApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        //[Authorize(Roles = "Administrators")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            if (_context.Projects == null)
            {
                return NotFound();
            }
            return await _context.Projects.ToListAsync();
        }


        [HttpGet(nameof(ByMe))]
        public async Task<ActionResult<IEnumerable<Project>>> ByMe()
        {
            if (_context.Projects == null || _context.Users == null || User?.Identity == null)
            {
                return NotFound();
            }
            var users = await _context.Users.ToListAsync(); 
            var myUser = users.SingleOrDefault(user => user.Email == User.Identity.Name);
            if (myUser == null)
            {
                return NotFound();
            }

            return await _context.Projects.Where(project => project.CreatorId == myUser.Id).ToListAsync();
        }

        //[HttpGet("{id}")]
        //[Authorize(Roles = "Administrators")]
        //public async Task<ActionResult<Project>> GetProject(Guid id)
        //{
        //    if (_context.Projects == null)
        //    {
        //        return NotFound();
        //    }
        //    var project = await _context.Projects.FindAsync(id);

        //    if (project == null)
        //    {
        //        return NotFound();
        //    }

        //    return project;
        //}

        //[HttpPut("{id}")]
        //[Authorize(Roles = "Administrators")]
        //public async Task<IActionResult> PutProject(Guid id, Project project)
        //{
        //    if (id != project.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(project).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ProjectExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        /// <summary>
        /// Assigns a user to a project.
        /// </summary>
        /// <param name="id">project id</param>
        /// <param name="userIds">user id to assign</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     Put /api/Projects/1/1
        ///
        /// </remarks>
        /// <returns>response</returns>
        /// <response code="200">On succesful assignment</response>
        /// <response code="404">If project or users are missing</response>
        [HttpPut("{id}/{userId}")]
        public async Task<IActionResult> AssignUsersToMyProject(Guid id, Guid userId)
        {
            if (_context.Projects == null || _context.Users == null || User?.Identity == null)
            {
                return NotFound();
            }

            var myUser = _context.Users.Single(user => user.Name == User.Identity.Name);
            var myProject = _context.Projects.Single(project => project.Id == id);
            var assignedUser = _context.Users.SingleOrDefault(user => user.Id == userId);

            if (myUser == null || myProject == null || assignedUser == null)
            {
                return NotFound();
            }

            myProject.AssignedUsers.Add(assignedUser);
            _context.Entry(myProject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'WebApiContext.Project' is null.");
            }
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = project.Id }, project);
        }

        //[HttpDelete("{id}")]
        //[Authorize(Roles = "Administrators")]
        //public async Task<IActionResult> DeleteProject(Guid id)
        //{
        //    if (_context.Projects == null)
        //    {
        //        return NotFound();
        //    }
        //    var project = await _context.Projects.FindAsync(id);
        //    if (project == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Projects.Remove(project);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool ProjectExists(Guid id)
        {
            return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
