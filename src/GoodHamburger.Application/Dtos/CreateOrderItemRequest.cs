namespace GoodHamburger.Application.Dtos;

public record CreateOrderItemRequest(int MenuItemId, int Quantity);