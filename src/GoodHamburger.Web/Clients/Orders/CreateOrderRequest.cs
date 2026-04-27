namespace GoodHamburger.Web.Clients.Orders;

public record CreateOrderRequest(List<CreateOrderItemRequest> Items);
