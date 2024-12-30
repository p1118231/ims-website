using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using imsWeb.Data;
using imsWeb.Models;
using imsWeb.Services.ProductRepo;
using imsWeb.Services.HttpServices;

namespace imsWeb.Controllers;

public class CartController:Controller{

    private readonly ILogger<CartController> _logger;
    private readonly IProductService _productService;

    public CartController(ILogger<CartController> logger, IProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    [HttpGet]
    public IActionResult Index()
    {
            // Retrieve the cart from the session
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Pass the cart to the view
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddToBasket(int productId, int quantity)
    {
        var product = await _productService.GetProductByIdAsync(productId);

        if (product == null)
        {
            return NotFound();
        }

        if (quantity > product.Quantity)
        {
            TempData["Error"] = "The requested quantity exceeds available stock.";
            return RedirectToAction("Details", new { id = productId });
        }

        // Update product stock
        product.Quantity -= quantity;

        // Retrieve or initialize the cart
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

        // Check if product is already in the cart
        var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = productId,
                ProductName = product.Name,
                Quantity = quantity,
                Price = product.Price,
                Image = product.ImageUrl
            });
        }

        // Update session with the updated cart
        HttpContext.Session.SetObjectAsJson("Cart", cart);

        // Update cart count in session
        HttpContext.Session.SetInt32("CartCount", cart.Sum(c => c.Quantity));

        await _productService.SaveChangesAsync();

        TempData["Success"] = $"{quantity} {product.Name}(s) added to your basket!";

        return RedirectToAction("Index", "Home");
    }

}

