using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace imsWeb.Models.Orders;

public class Order
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int Id { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime OrderDate { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalPrice { get; set; }
    // Navigation Property for Order Items
    public ICollection<OrderItem>? OrderItems { get; set; }
}
