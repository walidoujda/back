using back.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace back.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly TestContext _context;

        public ProductController(TestContext context)
        {
            _context = context;
        }

        // Fix for CS1061: Replace the incorrect usage of `ToListAsync` with `ToList`
        // Explanation: `ToListAsync` is an asynchronous method available for IQueryable, but `products` is a List<ProductResponse>, which does not support asynchronous operations. Use `ToList` instead.

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts()
        {
            if (_context.Products == null)
                return NotFound("No products found.");

            List<ProductResponse> products = new List<ProductResponse>();
            foreach (var product in await _context.Products.ToListAsync())
            {
                ProductResponse productResponse = new ProductResponse
                {
                    Id = product.Id,
                    Code = product.Code,
                    Name = product.Name,
                    Description = product.Description,
                    Image = product.Image,
                    Price = product.Price,
                    Quantity = product.Quantity - CartStore.GetReservedQuantity(product.Id),
                    inventoryStatus = product.Quantity - CartStore.GetReservedQuantity(product.Id) > 0 ? product.Quantity < 5 ? "LOWSTOCK" : "INSTOCK" : "OUTOFSTOCK",
                    InternalReference = product.InternalReference,
                    ShellId = product.ShellId,
                    Rating = product.Rating,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                };
                products.Add(productResponse);
            }

            return Ok(products); // Changed return type to ActionResult<IEnumerable<ProductResponse>> and used Ok() to wrap the response.
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponse>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            ProductResponse productResponse = new ProductResponse
            {
                Id = product.Id,
                Code = product.Code,
                Name = product.Name,
                Description = product.Description,
                Image = product.Image,
                Price = product.Price,
                Quantity = product.Quantity,
                inventoryStatus = product.Quantity > 0 ? product.Quantity < 5 ? "LOWSTOCK" : "INSTOCK" : "OUTOFSTOCK",
                InternalReference = product.InternalReference,
                ShellId = product.ShellId,
                Rating = product.Rating,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };

            return Ok(productResponse);
        }

        // POST: api/products
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Product>> CreateProduct([FromForm] Product product, [FromForm] IFormFile? imageFile)
        {

            if (imageFile != null && imageFile.Length > 0)
            {
                var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(imagesPath))
                    Directory.CreateDirectory(imagesPath);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var filePath = Path.Combine(imagesPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                product.Image = $"/images/{fileName}";
            }

            product.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
            _context.Products.Add(product); 
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // PUT: api/products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
                return NotFound();

            // Mise à jour des champs
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Quantity = product.Quantity;
            existingProduct.Rating = product.Rating;
            existingProduct.Code = product.Code;
            existingProduct.InternalReference = product.InternalReference;
            existingProduct.Image = product.Image;
            existingProduct.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
    public partial class ProductResponse
    {
        public int Id { get; set; }

        public string? Code { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }

        public float? Price { get; set; }

        public int? Quantity { get; set; }

        public string? InternalReference { get; set; }

        public string? ShellId { get; set; }

        public string? inventoryStatus { get; set; }

        public int? Rating { get; set; }

        public DateOnly? CreatedAt { get; set; }

        public DateOnly? UpdatedAt { get; set; }
    }
}
