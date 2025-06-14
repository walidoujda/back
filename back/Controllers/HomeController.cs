using back.Models; // Assurez-vous que le namespace correspond à votre projet
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace back.Controllers
{
    [ApiController]
    [Route("/")] // Ceci indique que c’est la racine "/"
    public class HomeController : ControllerBase
    {
        private readonly TestContext _context;
        public HomeController(TestContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            Product product = new Product
            {
                Name = "Produit Test",
                Description = "Ceci est un produit de test.",
                Price = 19.99f,
                Quantity = 100,
                Rating = 5,
                Code = "TEST123",
                InternalReference = "INT-001",
                Image = "https://example.com/image.jpg",
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            };
            _context.Products.Add(product);
            _context.SaveChanges();
            return Ok("Bienvenue sur l'API produits. 🚀 Utilise /products pour gérer tes produits.");
        }
    }
}