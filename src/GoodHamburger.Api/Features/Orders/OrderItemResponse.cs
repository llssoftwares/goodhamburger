namespace GoodHamburger.Api.Features.Orders;

public record OrderItemResponse(int MenuItemId, string MenuItemName, decimal UnitPrice, int Quantity);