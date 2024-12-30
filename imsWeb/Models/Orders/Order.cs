namespace imsWeb.Models.Orders;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalPrice { get; set; }

    // Navigation Property for Order Items
    public ICollection<OrderItem>? OrderItems { get; set; }
}
