using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace imsWeb.Models.Orders;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalPrice { get; set; }
    // Navigation Property for Order Items
    public ICollection<OrderItem>? OrderItems { get; set; }
}
