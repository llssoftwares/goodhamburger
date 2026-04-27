namespace GoodHamburger.Application.Dtos;

public record UpdateOrderRequest(List<CreateOrderItemRequest> Items);
