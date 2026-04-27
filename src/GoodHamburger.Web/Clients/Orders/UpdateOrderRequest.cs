namespace GoodHamburger.Web.Clients.Orders;

public record UpdateOrderRequest(List<CreateOrderItemRequest> Items);
