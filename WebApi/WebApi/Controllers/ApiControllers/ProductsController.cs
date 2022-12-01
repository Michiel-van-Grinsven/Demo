using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models.DataModels;

namespace WebApi.Controllers.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly WebApiContext _context;

        public ProductsController(WebApiContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetProducts()
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            return await _context.Products.Select(product => new ProductReadDto(product)).ToListAsync();
        }

        // GET: api/Products
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetMyProducts()
        {
            if (_context.Products == null || _context.Users == null || User.Identity == null)
            {
                return NotFound();
            }

            var myUser = _context.Users.SingleOrDefault(user => user.Email == User.Identity.Name);

            if (myUser == null)
            {
                return NotFound();
            }
            return await _context.Products.Where(product => product.CreatorId == myUser.Id).Select(product => new ProductReadDto(product)).ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductReadDto>> GetProduct(Guid id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return new ProductReadDto(product);
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        //[Authorize(Roles = "Administrators")]
        public async Task<IActionResult> PutProduct(Guid id, ProductUpdateDto dto)
        {
            var product = new Product(dto)
            {
                Id = id,
                UpdatedDate = DateTime.Now,
            };

            product.Creator = _context.Users.SingleOrDefault(user => user.Id == product.CreatorId);
            if (product.Creator == null)
            {
                return Problem("Creator does not exist.");
            }
            product.AssignedProjects = _context.Projects.Where(project => dto.AssignedProjects.Contains(project.Id)).ToList();

            _context.Entry(product).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductUpdateDto>> PostProduct(ProductCreateDto dto)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'WebApiContext.Products' is null.");
            }

            var product = new Product(dto);
            product.Creator = _context.Users.SingleOrDefault(user => user.Id == product.CreatorId);
            if (product.Creator == null)
            {
                return Problem("Creator does not exist.");
            }
            product.AssignedProjects = _context.Projects.Where(project => dto.AssignedProjects.Contains(project.Id)).ToList();

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, new ProductReadDto(product));
        }

        // POST: api/Products/my
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("my")]
        public async Task<ActionResult<ProductUpdateDto>> PostMyProduct(ProductUpdateDto dto)
        {
            if (_context.Products == null || _context.Users == null || User.Identity == null)
            {
                return Problem("Entity set or User identity is null.");
            }

            var product = new Product(dto);
            product.Creator = _context.Users.SingleOrDefault(user => user.Email == User.Identity.Name);
            if (product.Creator == null)
            {
                return Problem("Creator does not exist.");
            }
            product.AssignedProjects = _context.Projects.Where(project => dto.AssignedProjects.Contains(project.Id)).ToList();

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, new ProductReadDto(product));
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(Guid id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
