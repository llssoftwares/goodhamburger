using GoodHamburger.Application.Dtos;
using GoodHamburger.Application.Services;
using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Api.Features.Orders;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders");

        group.MapPost("/", CreateOrder);
        group.MapGet("/", GetOrders);
        group.MapGet("/{id}", GetOrder);
        group.MapPut("/{id}", UpdateOrder);
        group.MapDelete("/{id}", DeleteOrder);

        return app;
    }

    private static async Task<IResult> CreateOrder(CreateOrderRequest request, OrderService orderService)
    {
        var (order, errors) = await orderService.CreateOrderAsync(request);

        if (errors.Any())
            return Results.ValidationProblem(ToValidationErrors(errors));

        var response = MapOrderToResponse(order!);
        return Results.Created($"/api/orders/{order!.Id}", response);
    }

    private static async Task<IResult> GetOrders(OrderService orderService)
    {
        var orders = await orderService.GetAllOrdersAsync();
        var response = orders.Select(MapOrderToResponse).ToList();
        return Results.Ok(response);
    }

    private static async Task<IResult> GetOrder(int id, OrderService orderService)
    {
        var order = await orderService.GetOrderByIdAsync(id);

        if (order is null)
            return Results.Problem(
                title: "Pedido não encontrado",
                detail: $"Pedido com id {id} não existe",
                statusCode: StatusCodes.Status404NotFound
            );

        var response = MapOrderToResponse(order);
        return Results.Ok(response);
    }

    private static async Task<IResult> UpdateOrder(int id, UpdateOrderRequest request, OrderService orderService)
    {
        var (order, errors) = await orderService.UpdateOrderAsync(id, request);

        if (errors.Any())
            return Results.ValidationProblem(ToValidationErrors(errors));

        var response = MapOrderToResponse(order!);
        return Results.Ok(response);
    }

    private static async Task<IResult> DeleteOrder(int id, OrderService orderService)
    {
        var success = await orderService.DeleteOrderAsync(id);

        if (!success)
            return Results.Problem(
                title: "Pedido não encontrado",
                detail: $"Pedido com id {id} não existe",
                statusCode: StatusCodes.Status404NotFound
            );

        return Results.NoContent();
    }

    private static Dictionary<string, string[]> ToValidationErrors(IEnumerable<string> errors)
    {        
        return new Dictionary<string, string[]>
        {
            ["order"] = [.. errors]
        };
    }

    private static OrderResponse MapOrderToResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Status = order.Status.ToString(),
            Items = [.. order.Items.Select(i => new OrderItemResponse(
                i.MenuItemId,
                i.MenuItem.Name,
                i.UnitPrice,
                i.Quantity
            ))],
            Subtotal = order.Subtotal,
            DiscountPercentage = order.DiscountPercentage,
            Discount = order.Discount,
            Total = order.Total
        };
    }
}