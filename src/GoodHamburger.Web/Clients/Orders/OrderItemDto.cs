namespace GoodHamburger.Web.Clients.Orders;

public record OrderItemDto(int MenuItemId, string MenuItemName, decimal UnitPrice, int Quantity);
