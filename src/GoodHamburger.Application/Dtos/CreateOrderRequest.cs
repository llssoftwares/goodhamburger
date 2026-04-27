namespace GoodHamburger.Application.Dtos;

public record CreateOrderRequest(List<CreateOrderItemRequest> Items);
