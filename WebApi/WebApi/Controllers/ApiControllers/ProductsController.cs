using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Security.Claims;
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

        private bool ProductExists(Guid id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private IIncludableQueryable<Product, ICollection<Project>> GetProductsIncludingNavigation()
        {
            return _context.Products
                .Include(p => p.Creator)
                .Include(p => p.AssignedProjects);
        }

        /// <summary>
        /// Get all products.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     Put /api/Products/
        /// </remarks>
        /// <returns>A list of products.</returns>
        /// <response code="404">No products found.</response>
        [HttpGet, Authorize]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetProducts()
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            return await GetProductsIncludingNavigation()
                .Select(product => new ProductReadDto(product)).ToListAsync();
        }

        /// <summary>
        /// Get all my products.
        /// </summary>
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetMyProducts()
        {
            if (_context.Products == null || _context.Users == null || User.Identity == null)
            {
                return NotFound();
            }

            var myUser = _context.Users
                .SingleOrDefault(user => user.Id.ToString()
                .Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            if (myUser == null)
            {
                return NotFound();
            }

            return await GetProductsIncludingNavigation()
                .Where(product => product.CreatorId == myUser.Id)
                .Select(product => new ProductReadDto(product)).ToListAsync();
        }

        /// <summary>
        /// Get product by id.
        /// </summary>
        /// <param name="id">projtect id</param>
        /// <remarks>
        /// Sample request:
        ///     Put /api/Products/{guid}
        /// </remarks>
        /// <returns>A product.</returns>
        /// <response code="404">No  product found.</response>
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

        /// <summary>
        /// Overwrite a product.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize("Admin")]
        public async Task<IActionResult> PutProduct(Guid id, ProductCreateDto dto)
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
            var projects = await _context.Projects.Where(project => dto.AssignedProjects.Contains(project.Id)).ToListAsync();
            product.AssignedProjects = projects;

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

        /// <summary>
        /// Edit my product.
        /// </summary>
        [HttpPut("{id}/my")]
        public async Task<IActionResult> EditMyProduct(Guid id, ProductCreateDto dto)
        {
            var product = new Product(dto)
            {
                Id = id,
                UpdatedDate = DateTime.Now,
            };

            var myUser = _context.Users
                .SingleOrDefault(user => user.Id.ToString()
                .Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            if (myUser == null)
            {
                return NotFound();
            }

            product.Creator = _context.Users.SingleOrDefault(user => user.Id == product.CreatorId);
            var projects = await _context.Projects.Where(project => dto.AssignedProjects.Contains(project.Id)).ToListAsync();
            product.AssignedProjects = projects;

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

        /// <summary>
        /// Create a product.
        /// </summary>
        [HttpPost]
        [Authorize("Admin")]
        public async Task<ActionResult<ProductCreateDto>> PostProduct(ProductCreateDto dto)
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

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, new ProductReadDto(product));
        }

        /// <summary>
        /// Create a new product for me.
        /// </summary>
        [HttpPost("my")]
        public async Task<ActionResult<ProductUpdateDto>> PostMyProduct(ProductUpdateDto dto)
        {
            if (_context.Products == null || _context.Users == null || User.Identity == null)
            {
                return Problem("Entity set or User identity is null.");
            }

            var product = new Product(dto)
            {
                Creator = _context.Users.SingleOrDefault(user => user.UserName == User.Identity.Name)
            };
            if (product.Creator == null)
            {
                return Problem("Creator does not exist.");
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, new ProductReadDto(product));
        }

        /// <summary>
        /// Delete a product.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize("Admin")]
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

        /// <summary>
        /// Delete my product.
        /// </summary>
        [HttpDelete("{id}/my")]
        public async Task<IActionResult> DeleteMyProduct(Guid id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }

            var myUser = _context.Users
                .SingleOrDefault(user => user.Id.ToString()
                .Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            if (myUser == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            if (product.CreatorId != myUser.Id)
            {
                return Problem("Access denied.");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
