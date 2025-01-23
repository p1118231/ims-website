
using imsWeb.Data;
using Microsoft.EntityFrameworkCore;
using imsWeb.Models.Orders;
using imsWeb.Services.OrderRepo;
using imsWeb.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace imsWeb.Services.ProductRepo
{


    public class OrderService : IOrderService
    {

        private readonly ProductContext _context;

            public OrderService(ProductContext context)
        {
            _context = context;
        }

        public async Task<bool> AddOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Order?> GetOrderByIdAsync(int? id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            return (IEnumerable<Order>)await _context.Orders.ToListAsync();
        }
        
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateOrder(Order order)
        {
            // Save changes
         _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return true;
        }

        public List<OrderViewModel> GetAllOrders()
        {
            return _context.Orders
                .Select(order => new OrderViewModel
                {
                    OrderId = order.Id,
                    OrderDate = order.OrderDate,
                    TotalPrice = order.TotalPrice,
                    Items = order.OrderItems.Select(item => new OrderItemViewModel
                    {
                        ProductName = item.Product.Name,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Total = item.Quantity * item.Price,
                        ProductImage = item.Product.ImageUrl // Assuming Product has an ImageUrl
                    }).ToList()
                })
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }
    
    }
}
