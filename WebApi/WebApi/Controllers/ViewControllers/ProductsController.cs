using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Security.Claims;
using WebApi.Data;
using WebApi.Models.DataModels;

namespace WebApi.Controllers.ViewControllers
{
    public class ProductsController : Controller
    {

        static readonly List<KeyValuePair<string, int>> Units = new List<KeyValuePair<string, int>>() {
            new KeyValuePair<string, int>("g", 1),
            new KeyValuePair<string, int>("kg", 1000),
            new KeyValuePair<string, int>("t", 1000000)
        };

        private readonly WebApiContext _context;

        public ProductsController(WebApiContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var webApiContext = _context.Products
                .Include(p => p.Creator)
                .Include(p => p.AssignedProjects);
            return View(await webApiContext.ToListAsync());
        }

        public IActionResult Create()
        {
            var units = new SelectList(Units.Select(unit => unit.Key));
            ViewData["Units"] = units;
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["AssignedProjects"] = new SelectList(_context.Projects, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Unit,WeightInGrams,CarbonOutputPerGram")] ProductCreateDto dto)
        {
            var product = new Product(dto);
            if (ModelState.IsValid)
            {
                product.WeightInGrams = decimal.Parse(dto.WeightInGrams, CultureInfo.InvariantCulture);
                product.CarbonOutputPerGram = decimal.Parse(dto.CarbonOutputPerGram, CultureInfo.InvariantCulture);
                // TODO Solve localization
                product.WeightInGrams *= Units.Single(unit => unit.Key == dto.Unit).Value;
                product.Id = Guid.NewGuid();
                product.CreatedDate = DateTime.Now;
                product.UpdatedDate = DateTime.Now;
                product.CreatorId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // todo extract
            var units = new SelectList(Units.Select(unit => unit.Key));
            ViewData["Units"] = units;
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["AssignedProjects"] = new SelectList(_context.Projects, "Id", "Id");
            return View(product);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "FirstName", product.CreatorId);
            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,WeightInGrams,CarbonOutputPerGram")] Product product)
        {
            var oldProduct = await _context.Products.AsNoTracking()
                .SingleOrDefaultAsync(product => product.Id == id);
            var user = await _context.Users
                .SingleOrDefaultAsync(user => user.Id == new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            product.Id = id;
            product.Creator = user;
            product.CreatorId = user.Id;

            if (id != oldProduct.Id || oldProduct.CreatorId != user.Id)
            {
                return NotFound();
            }

            // todo extract
            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "FirstName", product.CreatorId);
            return View(product);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Creator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'WebApiContext.Products' is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(Guid id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
