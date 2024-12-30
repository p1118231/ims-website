using System;
using System.Collections.Generic;
using imsWeb.Models.Orders;

namespace imsWeb.Models.Orders
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemViewModel>? Items { get; set; }
    }
}
