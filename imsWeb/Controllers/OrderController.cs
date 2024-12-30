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
using imsWeb.Services.OrderRepo;

namespace imsWeb.Controllers;

public class OrderController:Controller{

    private readonly ILogger<OrderController> _logger;
    private readonly IOrderService _orderService;

    public OrderController(ILogger<OrderController> logger, IOrderService orderService)
    {
        _logger = logger;
        _orderService = orderService;
    }

     [HttpGet]
    public IActionResult Index()
    {
        // Fetch orders from the database
        var orders = _orderService.GetAllOrders();
        return View(orders);
    }

    [HttpGet]
    public IActionResult OrderConfirmation()
    {
        return View();
    }



}