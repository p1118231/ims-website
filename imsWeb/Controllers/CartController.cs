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
using imsWeb.Models.Orders;
using imsWeb.Services.OrderRepo;

namespace imsWeb.Controllers;

public class CartController:Controller{

    private readonly ILogger<CartController> _logger;
    private readonly IProductService _productService;

    private readonly IOrderService _orderService;



    public CartController(ILogger<CartController> logger, IProductService productService, IOrderService orderService)
    {
        _logger = logger;
        _productService = productService;
        _orderService = orderService;
    }

    [HttpGet]
    //show the items in the cart
    public IActionResult Index()
    {
            // Retrieve the cart from the session
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Pass the cart to the view
        return View(cart);
    }

    [HttpPost]
    //add to basket
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

    [HttpPost]
    //remove item from the cart
    public IActionResult RemoveItem(int productId)
    {
        // Retrieve the cart from the session
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

        // Find the item to remove
        var itemToRemove = cart.FirstOrDefault(c => c.ProductId == productId);
        if (itemToRemove != null)
        {
            cart.Remove(itemToRemove);
        }

        // Update the session
        HttpContext.Session.SetObjectAsJson("Cart", cart);

        // Redirect back to the Cart view
        return RedirectToAction("Index","Cart");
    }

    [HttpGet]
    //direct the user to the checkout 
    public IActionResult CheckOut()
    {
        // Retrieve the cart from the session
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

        if (cart == null || !cart.Any())
        {
            TempData["Error"] = "Your cart is empty. Add items before proceeding to checkout.";
            return RedirectToAction("Cart");
        }

        // Pass the cart to the view
        return View(cart);
    }

    [HttpPost]
    //direct user to place an order 
    public async Task<IActionResult> PlaceOrder()
    {
        // Retrieve the cart from the session
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

        if (cart == null || !cart.Any())
        {
            TempData["Error"] = "Your cart is empty. Add items before placing an order.";
            return RedirectToAction("Cart", "Cart");
        }

        // Create a new order
        var order = new Order
        {
            OrderDate = DateTime.UtcNow,
            TotalPrice = cart.Sum(c => c.Quantity * c.Price),
            OrderItems = cart.Select(c => new OrderItem
            {
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                Price = c.Price
            }).ToList()
        };

        // Add the order to the database
        await _orderService.AddOrder(order);
        await _orderService.SaveChangesAsync();

        // Update the product quantities in the database
        foreach (var item in cart)
        {
            var product = await _productService.GetProductByIdAsync(item.ProductId);
            if (product == null)
            {
                TempData["Error"] = $"Product '{item.ProductName}' no longer exists.";
                return RedirectToAction("Cart", "Cart");
            }

            if (product.Quantity < item.Quantity)
            {
                TempData["Error"] = $"Not enough stock for '{item.ProductName}'. Available: {product.Quantity}.";
                return RedirectToAction("Cart", "Cart");
            }

            Console.Write("Product Quantity: " + product.Quantity);

            // Deduct the ordered quantity
            product.Quantity -= item.Quantity;

            // Update the product in the database
            await _productService.UpdateProduct(product);
            Console.Write("Product Quantity: " + product.Quantity);
        }

        // Save changes to the database
        await _productService.SaveChangesAsync();
        

        // Clear the cart after placing the order
        HttpContext.Session.Remove("Cart");
        HttpContext.Session.SetInt32("CartCount", 0);

        // Set the success message
        TempData["Success"] = "Your order has been placed successfully! Thank you for shopping with us.";

        return RedirectToAction("OrderConfirmation");
    }


    [HttpGet]
    //show a confirmation after placing an order 
    public IActionResult OrderConfirmation()
    {
        return View();
    }

}

