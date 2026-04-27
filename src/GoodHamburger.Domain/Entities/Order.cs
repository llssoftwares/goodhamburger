namespace GoodHamburger.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public OrderStatus Status { get; set; }    
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal Total { get; set; }    
    public ICollection<OrderItem> Items { get; set; } = [];
}