using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using imsWeb.Models;
using imsWeb.Models.Orders;

namespace imsWeb.Services.OrderRepo;

public interface IOrderService{

    Task<IEnumerable<Order>> GetOrders();

    Task<Order?> GetOrderByIdAsync(int? id);

    Task SaveChangesAsync();

    Task<bool> UpdateOrder(Order order);

    Task<bool> AddOrder(Order order);

    List<OrderViewModel> GetAllOrders();


    
}