namespace GoodHamburger.Web.Clients.Orders;

public record OrderDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
}
