using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Web.Clients.Orders;

public class OrderClient(HttpClient httpClient, ILogger<OrderClient> logger)
{
    public async Task<List<OrderDto>?> GetOrdersAsync()
    {
        try
        {
            return await httpClient.GetFromJsonAsync<List<OrderDto>>("/api/orders");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching orders");
            throw;
        }
    }

    public async Task<OrderDto?> GetOrderAsync(int id)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<OrderDto>($"/api/orders/{id}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching order {OrderId}", id);
            throw;
        }
    }

    public async Task<(OrderDto? Order, List<string> Errors)> CreateOrderAsync(CreateOrderRequest request)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("/api/orders", request);

            if (response.IsSuccessStatusCode)
            {
                var order = await response.Content.ReadFromJsonAsync<OrderDto>();
                return (order, new List<string>());
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

                var errors = problem?.Errors?
                    .SelectMany(e => e.Value)
                    .ToList() ?? [];

                return (null, errors);
            }

            var genericProblem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            return (null, new List<string>
        {
            genericProblem?.Detail ?? "Erro ao criar pedido"
        });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating order");
            return (null, new List<string> { "Erro ao criar pedido" });
        }
    }

    public async Task<(OrderDto? Order, List<string> Errors)> UpdateOrderAsync(int id, UpdateOrderRequest request)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync($"/api/orders/{id}", request);

            if (response.IsSuccessStatusCode)
            {
                var order = await response.Content.ReadFromJsonAsync<OrderDto>();
                return (order, new List<string>());
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

                var errors = problem?.Errors?
                    .SelectMany(e => e.Value)
                    .ToList() ?? new List<string>();

                return (null, errors);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

                return (null, new List<string>
            {
                problem?.Detail ?? "Pedido não encontrado"
            });
            }

            var genericProblem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            return (null, new List<string>
        {
            genericProblem?.Detail ?? "Erro ao atualizar pedido"
        });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating order {OrderId}", id);
            return (null, new List<string> { "Erro ao atualizar pedido" });
        }
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"/api/orders/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting order {OrderId}", id);
            throw;
        }
    }
}