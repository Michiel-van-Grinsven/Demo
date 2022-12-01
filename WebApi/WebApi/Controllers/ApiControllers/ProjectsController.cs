using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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

        private bool ProjectExists(Guid id)
        {
            return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private IIncludableQueryable<Project, ICollection<Product>> GetProjectsIncludingNavigation()
        {
            return _context.Projects.Include(p => p.AssignedUsers).Include(p => p.AssignedProducts);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectReadDto>>> GetProjects()
        {
            if (_context.Projects == null)
            {
                return NotFound();
            }
            return await GetProjectsIncludingNavigation()
                .Select(project => new ProjectReadDto(project)).ToListAsync();
        }

        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<ProjectReadDto>>> GetMyProjects()
        {
            if (_context.Projects == null || _context.Users == null || User.Identity == null)
            {
                return NotFound();
            }

            var myUser = _context.Users.SingleOrDefault(user => user.UserName == User.Identity.Name);

            if (myUser == null)
            {
                return NotFound();
            }
            return await GetProjectsIncludingNavigation()
                .Where(project => project.CreatorId == myUser.Id)
                .Select(project => new ProjectReadDto(project)).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectReadDto>> GetProject(Guid id)
        {
            if (_context.Projects == null)
            {
                return NotFound();
            }
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return new ProjectReadDto(project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(Guid id, ProjectCreateDto dto)
        {
            var project = new Project(dto)
            {
                Id = id,
                UpdatedDate = DateTime.Now,
            };

            project.Creator = _context.Users.SingleOrDefault(user => user.Id == project.CreatorId);
            if (project.Creator == null)
            {
                return Problem("Creator does not exist.");
            }
            var users = await _context.Users.Where(user => dto.AssignedUsers.Contains(user.Id)).ToListAsync();
            project.AssignedUsers = users;
            var products = await _context.Products.Where(product => dto.AssignedProducts.Contains(product.Id)).ToListAsync();
            project.AssignedProducts = products;

            _context.Entry(project).State = EntityState.Modified;
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
        public async Task<ActionResult<ProjectCreateDto>> PostProject(ProjectCreateDto dto)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'WebApiContext.Projects' is null.");
            }

            var project = new Project(dto);
            project.Creator = _context.Users.SingleOrDefault(user => user.Id == project.CreatorId);
            if (project.Creator == null)
            {
                return Problem("Creator does not exist.");
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, new ProjectReadDto(project));
        }

        [HttpPost("my")]
        public async Task<ActionResult<ProjectUpdateDto>> PostMyProject(ProjectUpdateDto dto)
        {
            if (_context.Projects == null || _context.Users == null || User.Identity == null)
            {
                return Problem("Entity set or User identity is null.");
            }

            var project = new Project(dto)
            {
                Creator = _context.Users.SingleOrDefault(user => user.UserName == User.Identity.Name)
            };
            if (project.Creator == null)
            {
                return Problem("Creator does not exist.");
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, new ProjectReadDto(project));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            if (_context.Projects == null)
            {
                return NotFound();
            }
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
