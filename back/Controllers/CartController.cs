﻿using back.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back.Controllers
{
    public static class CartStore
    {
        // Correction de CA2211 : rendre le champ privé et ajouter une propriété publique pour l'accès  
        private static readonly Dictionary<int, List<CartItem>> _carts = new(); // Correction de IDE0028 : simplification de l'initialisation  

        public static Dictionary<int, List<CartItem>> Carts => _carts;

        public static int GetReservedQuantity(int productId)
        {
            return Carts.Values
                .SelectMany(items => items)
                .Where(item => item.ProductId == productId)
                .Sum(item => item.Quantity);
        }
    }

    [Authorize]
    [ApiController]
    [Route("api/cart/")]
    public class CartController : ControllerBase
    {
        private readonly TestContext _context;

        public CartController(TestContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetCart()
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return BadRequest("L'identifiant utilisateur est manquant ou invalide.");
            }

            var userId = int.Parse(userIdClaim.Value);
            CartStore.Carts.TryGetValue(userId, out var items);
            return Ok(items ?? new List<CartItem>());
        }

        [HttpPost("add")]
        public IActionResult AddToCart(CartItem item)
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return BadRequest("L'identifiant utilisateur est manquant ou invalide.");
            }

            var userId = int.Parse(userIdClaim.Value);

            if (!CartStore.Carts.ContainsKey(userId))
                CartStore.Carts[userId] = new List<CartItem>();

            Product? product = _context.Products.Find(item.ProductId);
            if (product == null || (product.Quantity ?? 0) - CartStore.GetReservedQuantity(item.ProductId) - item.Quantity < 0)
            {
                int maxCommande = 0;
                var existing = CartStore.Carts[userId].FirstOrDefault(ci => ci.ProductId == item.ProductId);
                if (existing != null)
                    maxCommande = (product?.Quantity ?? 0) - CartStore.GetReservedQuantity(item.ProductId) - existing.Quantity;
                else
                    maxCommande = (product?.Quantity ?? 0) - CartStore.GetReservedQuantity(item.ProductId);
                return BadRequest($"Stock épuisé pour ce produit. Max commande est : {maxCommande}");
            }

            var existingItem = CartStore.Carts[userId].FirstOrDefault(ci => ci.ProductId == item.ProductId);
            if (existingItem != null)
                existingItem.Quantity = item.Quantity;
            else
                CartStore.Carts[userId].Add(item);

            return Ok(CartStore.Carts[userId]);
        }

        [HttpDelete("clear")]
        public IActionResult ClearCart()
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return BadRequest("L'identifiant utilisateur est manquant ou invalide.");
            }

            var userId = int.Parse(userIdClaim.Value);
            CartStore.Carts.Remove(userId);
            return Ok();
        }

        [HttpPost("validate")]
        public IActionResult ValidateCart()
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return BadRequest("L'identifiant utilisateur est manquant ou invalide.");
            }

            var userId = int.Parse(userIdClaim.Value);

            if (!CartStore.Carts.TryGetValue(userId, out var items) || items.Count == 0)
            {
                return BadRequest("Le panier est vide.");
            }

            // Vérification du stock pour chaque produit
            foreach (var cartItem in items)
            {
                var product = _context.Products.Find(cartItem.ProductId);
                if (product == null)
                {
                    return BadRequest($"Produit avec l'ID {cartItem.ProductId} introuvable.");
                }

                if ((product.Quantity ?? 0) < cartItem.Quantity)
                {
                    return BadRequest($"Stock insuffisant pour le produit {product.Name} (ID: {product.Id}). Stock disponible : {product.Quantity}, demandé : {cartItem.Quantity}");
                }
            }

            // Création de la commande
            var order = new Order
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Status = 1, // À adapter selon ta logique (ex: 1 = "En attente")
                UserId = userId
            };
            _context.Orders.Add(order);
            _context.SaveChanges(); // Pour générer l'Id de la commande

            // Ajout des Orderitems et déduction du stock
            foreach (var cartItem in items)
            {
                var product = _context.Products.Find(cartItem.ProductId);

                // Création de l'Orderitem
                var orderItem = new Orderitem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity
                };
                _context.Orderitems.Add(orderItem);

                // Déduction du stock
                product.Quantity -= cartItem.Quantity;
                product.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
            }

            _context.SaveChanges();

            // Vide le panier après validation
            CartStore.Carts.Remove(userId);

            return Ok("Panier validé, commande créée et stock mis à jour.");
        }
    }
    public class CartItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
