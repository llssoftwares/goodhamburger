using GoodHamburger.Application.Dtos;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Application.Services;

public class OrderService(
    GoodHamburgerDbContext context,
    OrderValidationService validationService,
    DiscountCalculationService discountCalculationService)
{
    public async Task<(Order? Order, IList<string> Errors)> CreateOrderAsync(CreateOrderRequest request)
    {
        var menuItems = await context.MenuItems.ToListAsync();
        
        var validationErrors = validationService.ValidateOrderItems(request.Items, menuItems);

        if (validationErrors.Any())
        {
            return (null, validationErrors);
        }

        var order = new Order
        {
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Created,
            Items = []
        };
        
        foreach (var itemRequest in request.Items)
        {
            var menuItem = menuItems.FirstOrDefault(m => m.Id == itemRequest.MenuItemId);
            if (menuItem == null)
            {
                return (null, new[] { $"Item {itemRequest.MenuItemId} não encontrado no cardápio." }.ToList());
            }

            if (!menuItem.IsAvailable)
            {
                return (null, new[] { $"Item {menuItem.Name} não está disponível." }.ToList());
            }

            order.Items.Add(new OrderItem
            {
                MenuItemId = menuItem.Id,
                MenuItem = menuItem,
                UnitPrice = menuItem.Price,
                Quantity = itemRequest.Quantity
            });
        }
        
        CalculateOrderTotals(order);

        context.Orders.Add(order);
        await context.SaveChangesAsync();

        return (order, new List<string>());
    }
    
    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await context.Orders
            .Include(o => o.Items)
            .ThenInclude(oi => oi.MenuItem)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await context.Orders
            .Include(o => o.Items)
            .ThenInclude(oi => oi.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
    
    public async Task<(Order? Order, IList<string> Errors)> UpdateOrderAsync(int id, UpdateOrderRequest request)
    {
        var order = await context.Orders
            .Include(o => o.Items)
            .ThenInclude(oi => oi.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return (null, new[] { "Pedido não encontrado." }.ToList());
        }

        if (order.Status != OrderStatus.Created)
        {
            return (null, new[] { "Apenas pedidos com status 'Criado' podem ser atualizados." }.ToList());
        }

        var menuItems = await context.MenuItems.ToListAsync();
        
        var validationErrors = validationService.ValidateOrderItems(request.Items, menuItems);

        if (validationErrors.Any())
        {
            return (null, validationErrors);
        }

        order.Items.Clear();
        
        foreach (var itemRequest in request.Items)
        {
            var menuItem = menuItems.FirstOrDefault(m => m.Id == itemRequest.MenuItemId);
            if (menuItem == null)
            {
                return (null, new[] { $"Item {itemRequest.MenuItemId} não encontrado no cardápio." }.ToList());
            }

            if (!menuItem.IsAvailable)
            {
                return (null, new[] { $"Item {menuItem.Name} não está disponível." }.ToList());
            }

            order.Items.Add(new OrderItem
            {
                MenuItemId = menuItem.Id,
                MenuItem = menuItem,
                UnitPrice = menuItem.Price,
                Quantity = itemRequest.Quantity
            });
        }

        order.UpdatedAt = DateTime.UtcNow;
        
        CalculateOrderTotals(order);

        context.Orders.Update(order);
        await context.SaveChangesAsync();

        return (order, new List<string>());
    }
    
    public async Task<bool> DeleteOrderAsync(int id)
    {
        var order = await context.Orders.FindAsync(id);
        if (order == null)
        {
            return false;
        }

        context.Orders.Remove(order);
        await context.SaveChangesAsync();

        return true;
    }
    
    private void CalculateOrderTotals(Order order)
    {        
        order.Subtotal = order.Items.Sum(i => i.UnitPrice * i.Quantity);
        
        var (discountPercentage, _) = discountCalculationService.CalculateDiscount(order.Items);
        order.DiscountPercentage = discountPercentage;
        order.Discount = order.Subtotal * (discountPercentage / 100m);
        
        order.Total = order.Subtotal - order.Discount;
    }
}