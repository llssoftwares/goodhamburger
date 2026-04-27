using GoodHamburger.Application.Dtos;
using GoodHamburger.Application.Services;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Tests.Application;

public class OrderServiceTests
{
    private readonly GoodHamburgerDbContext _context;
    private readonly OrderService _orderService;
    private readonly OrderValidationService _validationService;
    private readonly DiscountCalculationService _discountCalculationService;

    public OrderServiceTests()
    {
        var options = new DbContextOptionsBuilder<GoodHamburgerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GoodHamburgerDbContext(options);
        _validationService = new OrderValidationService();
        _discountCalculationService = new DiscountCalculationService();
        _orderService = new OrderService(_context, _validationService, _discountCalculationService);
        
        SeedTestData();
    }

    private void SeedTestData()
    {
        _context.MenuItems.AddRange(
            new MenuItem { Id = 1, Name = "X Burger", Price = 5.00m, Type = MenuItemType.Sandwich },
            new MenuItem { Id = 2, Name = "X Egg", Price = 4.50m, Type = MenuItemType.Sandwich },
            new MenuItem { Id = 3, Name = "X Bacon", Price = 7.00m, Type = MenuItemType.Sandwich },
            new MenuItem { Id = 4, Name = "Batata frita", Price = 2.00m, Type = MenuItemType.Side },
            new MenuItem { Id = 5, Name = "Refrigerante", Price = 2.50m, Type = MenuItemType.Beverage }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task CreateOrderAsync_WithValidItems_CreatesOrder()
    {
        // Arrange
        var request = new CreateOrderRequest(
        [
            new(1, 1),
            new(4, 1),
            new(5, 1)
        ]);

        // Act
        var (order, errors) = await _orderService.CreateOrderAsync(request);

        // Assert
        Assert.Empty(errors);
        Assert.NotNull(order);
        Assert.Equal(3, order.Items.Count);
        Assert.Equal(9.50m, order.Subtotal); // 5 + 2 + 2.50
    }

    [Fact]
    public async Task CreateOrderAsync_WithSandwichBatataBeverage_Applies20PercentDiscount()
    {
        // Arrange
        var request = new CreateOrderRequest(
        [
            new(1, 1), // 5.00
            new(4, 1), // 2.00
            new(5, 1)  // 2.50
        ]);

        // Act
        var (order, errors) = await _orderService.CreateOrderAsync(request);

        // Assert
        Assert.Empty(errors);
        Assert.NotNull(order);
        Assert.Equal(20, order.DiscountPercentage);
        Assert.Equal(9.50m, order.Subtotal);
        Assert.Equal(1.90m, order.Discount); // 9.50 * 0.20
        Assert.Equal(7.60m, order.Total);    // 9.50 - 1.90
    }

    [Fact]
    public async Task CreateOrderAsync_WithSandwichBeverage_Applies15PercentDiscount()
    {
        // Arrange
        var request = new CreateOrderRequest(
        [
            new(2, 1), // 4.50
            new(5, 1)  // 2.50
        ]);

        // Act
        var (order, errors) = await _orderService.CreateOrderAsync(request);

        // Assert
        Assert.Empty(errors);
        Assert.NotNull(order);
        Assert.Equal(15, order.DiscountPercentage);
        Assert.Equal(7.00m, order.Subtotal);
        Assert.Equal(1.05m, order.Discount); // 7.00 * 0.15
        Assert.Equal(5.95m, order.Total);    // 7.00 - 1.05
    }

    [Fact]
    public async Task CreateOrderAsync_WithSandwichBatata_Applies10PercentDiscount()
    {
        // Arrange
        var request = new CreateOrderRequest(
        [
            new(3, 1), // 7.00
            new(4, 1)  // 2.00
        ]);

        // Act
        var (order, errors) = await _orderService.CreateOrderAsync(request);

        // Assert
        Assert.Empty(errors);
        Assert.NotNull(order);
        Assert.Equal(10, order.DiscountPercentage);
        Assert.Equal(9.00m, order.Subtotal);
        Assert.Equal(0.90m, order.Discount); // 9.00 * 0.10
        Assert.Equal(8.10m, order.Total);    // 9.00 - 0.90
    }

    [Fact]
    public async Task CreateOrderAsync_WithEmptyItems_ReturnsError()
    {
        // Arrange
        var request = new CreateOrderRequest([]);

        // Act
        var (order, errors) = await _orderService.CreateOrderAsync(request);

        // Assert
        Assert.Null(order);
        Assert.NotEmpty(errors);
        Assert.Contains("deve conter pelo menos um item", errors[0]);
    }

    [Fact]
    public async Task CreateOrderAsync_WithDuplicateSandwiches_ReturnsError()
    {
        // Arrange
        var request = new CreateOrderRequest(
        [
            new(1, 1),
            new(2, 1)
        ]);

        // Act
        var (order, errors) = await _orderService.CreateOrderAsync(request);

        // Assert
        Assert.Null(order);
        Assert.NotEmpty(errors);
        Assert.Contains("pode conter apenas um sanduíche", errors[0]);
    }

    [Fact]
    public async Task CreateOrderAsync_WithNonExistentMenuItem_ReturnsError()
    {
        // Arrange
        var request = new CreateOrderRequest(
        [
            new(999, 1)
        ]);

        // Act
        var (order, errors) = await _orderService.CreateOrderAsync(request);

        // Assert
        Assert.Null(order);
        Assert.NotEmpty(errors);
        Assert.Contains("não encontrado", errors[0]);
    }

    [Fact]
    public async Task GetAllOrdersAsync_ReturnsAllOrders()
    {
        // Arrange
        var request1 = new CreateOrderRequest([new(1, 1)]);
        var request2 = new CreateOrderRequest([new(2, 1)]);
        await _orderService.CreateOrderAsync(request1);
        await _orderService.CreateOrderAsync(request2);

        // Act
        var orders = await _orderService.GetAllOrdersAsync();

        // Assert
        Assert.Equal(2, orders.Count);
    }

    [Fact]
    public async Task GetOrderByIdAsync_WithValidId_ReturnsOrder()
    {
        // Arrange
        var request = new CreateOrderRequest([new(1, 1)]);
        var (createdOrder, _) = await _orderService.CreateOrderAsync(request);
        var orderId = createdOrder!.Id;

        // Act
        var order = await _orderService.GetOrderByIdAsync(orderId);

        // Assert
        Assert.NotNull(order);
        Assert.Equal(orderId, order.Id);
    }

    [Fact]
    public async Task GetOrderByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var order = await _orderService.GetOrderByIdAsync(999);

        // Assert
        Assert.Null(order);
    }

    [Fact]
    public async Task DeleteOrderAsync_WithValidId_DeletesOrder()
    {
        // Arrange
        var request = new CreateOrderRequest([new(1, 1)]);
        var (createdOrder, _) = await _orderService.CreateOrderAsync(request);
        var orderId = createdOrder!.Id;

        // Act
        var success = await _orderService.DeleteOrderAsync(orderId);
        var deletedOrder = await _orderService.GetOrderByIdAsync(orderId);

        // Assert
        Assert.True(success);
        Assert.Null(deletedOrder);
    }
}