using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq;
using System.Security.Claims;
using WebApi.Data;
using WebApi.Models.DataModels;

namespace WebApi.Controllers.ViewControllers
{
    public class ProjectsController : Controller
    {
        private readonly WebApiContext _context;

        public ProjectsController(WebApiContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            var projects = await GetProductContext().ToListAsync();
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["UserSortParm"] = sortOrder == "user" ? "user_desc" : "user";
            switch (sortOrder)
            {
                case "date_desc":
                    projects = projects.OrderByDescending(s => s.CreatedDate).ThenByDescending(s => s.Creator.Name).ToList();
                    break;
                case "date":
                    projects = projects.OrderBy(s => s.CreatedDate).ThenByDescending(s => s.Creator.Name).ToList();
                    break;

                case "user_desc":
                    projects = projects.OrderByDescending(s => s.AssignedUsers.Select(user => user.Id).Contains(new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier)))).ThenByDescending(s => s.Creator.Name).ToList();
                    break;
                case "user":
                    projects = projects.OrderBy(s => s.AssignedUsers.Select(user => user.Id).Contains(new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier)))).ThenByDescending(s => s.Creator.Name).ToList();

                    break;

                case "name_desc":
                    projects = projects.OrderByDescending(s => s.Name).ThenByDescending(s => s.Creator.Name).ToList();
                    break;
                default:
                    projects = projects.OrderBy(s => s.Name).ThenByDescending(s => s.Creator.Name).ToList();
                    break;
            }
            return View(projects);
        }

        private IIncludableQueryable<Project, ICollection<Product>> GetProductContext()
        {
            return _context.Projects
                .Include(p => p.Creator)
                .Include(p => p.AssignedUsers)
                .Include(p => p.AssignedProducts);
        }

        public IActionResult Create()
        {
            ViewData["CreatorId"] = new SelectList(_context.Users
                .OrderBy(x => x.LastName), "Id", "Name");

            var users = new SelectList(_context.Users.AsNoTracking()
                .Where(user => user.Id != new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                .OrderBy(x => x.LastName), "Id", "Name").ToList();
            users.Insert(0, new SelectListItem { Text = "[None]", Value = User.FindFirstValue(ClaimTypes.NameIdentifier) });
            ViewData["AssignedUsers"] = users;

            var products = new SelectList(_context.Products.AsNoTracking()
                .OrderBy(x => x.Name), "Id", "Name").ToList();
            products.Insert(0, new SelectListItem { Text = "[None]", Value = Guid.Empty.ToString() });
            ViewData["AssignedProducts"] = products;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,AssignedUsers,AssignedProducts")] ProjectCreateDto dto)
        {
            var project = new Project(dto);
            if (ModelState.IsValid)
            {
                project.Id = Guid.NewGuid();
                project.CreatedDate = DateTime.Now;
                project.UpdatedDate = DateTime.Now;
                project.CreatorId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
                project.Creator = await _context.Users.SingleOrDefaultAsync(user => user.Id == project.CreatorId);
                if (project.Creator == null)
                {
                    return Problem("Creator does not exist.");
                }
                var users = await _context.Users.Where(user => dto.AssignedUsers.Contains(user.Id) && project.CreatorId != user.Id).ToListAsync();
                project.AssignedUsers = users;
                var products = await _context.Products.Where(product => dto.AssignedProducts.Contains(product.Id)).ToListAsync();
                project.AssignedProducts = products;
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatorId"] = new SelectList(_context.Users.OrderBy(x => x.LastName), "Id", "Name");
            ViewData["AssignedUsers"] = new SelectList(_context.Users.OrderBy(x => x.LastName), "Id", "Name");
            ViewData["AssignedProducts"] = new SelectList(_context.Products.OrderBy(x => x.Name), "Id", "Name");
            return View(project);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["CreatorId"] = new SelectList(_context.Users
                .OrderBy(x => x.LastName), "Id", "Name");
            var users = _context.Users.AsNoTracking()
                .Where(user => user.Id != new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                .OrderBy(x => x.LastName);
            var items = new SelectList(users, "Id", "Name").ToList();
            items.Insert(0, (new SelectListItem { Text = "[None]", Value = Guid.Empty.ToString() }));
            ViewData["AssignedUsers"] = items;
            ViewData["AssignedProducts"] = new SelectList(_context.Products.OrderBy(x => x.Name), "Id", "Name");
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,AssignedUsers,AssignedProducts")] ProjectUpdateDto dto)
        {
            var project = await GetProductContext()
                .SingleOrDefaultAsync(project => project.Id == id);
            var user = await _context.Users
                .SingleOrDefaultAsync(user => user.Id == new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            project.Id = id;
            project.Creator = user;
            project.CreatorId = user.Id;
            project.UpdatedDate = DateTime.Now;
            if (id != project.Id || project.CreatorId != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (project.Creator == null)
                {
                    return Problem("Creator does not exist.");
                }
                var users = await _context.Users.Where(user => dto.AssignedUsers.Contains(user.Id) && project.CreatorId != user.Id).ToListAsync();
                project.AssignedUsers = users;
                var products = await _context.Products.Where(product => dto.AssignedProducts.Contains(product.Id)).ToListAsync();
                project.AssignedProducts = products;
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatorId"] = new SelectList(_context.Users.OrderBy(x => x.LastName), "Id", "Name");
            ViewData["AssignedUsers"] = new SelectList(_context.Users.OrderBy(x => x.LastName), "Id", "Name");
            ViewData["AssignedProducts"] = new SelectList(_context.Products.OrderBy(x => x.Name), "Id", "Name");
            return View(project);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Creator)
                .Include(p => p.AssignedProducts)
                .Include(p => p.AssignedUsers)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'WebApiContext.Projects' is null.");
            }
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Unassign(Guid? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Creator)
                .Include(p => p.AssignedProducts)
                .Include(p => p.AssignedUsers)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [HttpPost, ActionName("Unassign")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnassignConfirmed(Guid id)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'WebApiContext.Projects' is null.");
            }
            var project = await _context.Projects
                 .Include(p => p.Creator)
                 .Include(p => p.AssignedProducts)
                 .Include(p => p.AssignedUsers)
                 .FirstOrDefaultAsync(m => m.Id == id);
            var user = await _context.Users
             .SingleOrDefaultAsync(user => user.Id == new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            if (project != null && user != null)
            {
                project.AssignedUsers.Remove(user);
                _context.Update(project);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(Guid id)
        {
            return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
