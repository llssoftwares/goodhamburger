using GoodHamburger.Application.Dtos;
using GoodHamburger.Application.Services;
using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Tests.Application;

public class OrderValidationServiceTests
{
    private readonly OrderValidationService _validationService = new();
    private readonly List<MenuItem> _menuItems;

    public OrderValidationServiceTests()
    {
        _menuItems =
        [
            new() { Id = 1, Name = "X Burger", Price = 5.00m, Type = MenuItemType.Sandwich },
            new() { Id = 2, Name = "X Egg", Price = 4.50m, Type = MenuItemType.Sandwich },
            new() { Id = 3, Name = "X Bacon", Price = 7.00m, Type = MenuItemType.Sandwich },
            new() { Id = 4, Name = "Batata frita", Price = 2.00m, Type = MenuItemType.Side },
            new() { Id = 5, Name = "Refrigerante", Price = 2.50m, Type = MenuItemType.Beverage }
        ];
    }

    [Fact]
    public void ValidateOrderItems_WithValidItems_ReturnsNoErrors()
    {
        // Arrange
        var items = new List<CreateOrderItemRequest>
        {
            new(1, 1), // X Burger
            new(4, 1), // Batata frita
            new(5, 1)  // Refrigerante
        };

        // Act
        var errors = _validationService.ValidateOrderItems(items, _menuItems);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateOrderItems_WithEmptyItems_ReturnsError()
    {
        // Arrange
        var items = new List<CreateOrderItemRequest>();

        // Act
        var errors = _validationService.ValidateOrderItems(items, _menuItems);

        // Assert
        Assert.Single(errors);
        Assert.Contains("deve conter pelo menos um item", errors[0]);
    }

    [Fact]
    public void ValidateOrderItems_WithDuplicateSandwich_ReturnsError()
    {
        // Arrange
        var items = new List<CreateOrderItemRequest>
        {
            new(1, 1), // X Burger
            new(2, 1)  // X Egg (múltiplos sandwiches)
        };

        // Act
        var errors = _validationService.ValidateOrderItems(items, _menuItems);

        // Assert
        Assert.Single(errors);
        Assert.Contains("pode conter apenas um sanduíche", errors[0]);
    }

    [Fact]
    public void ValidateOrderItems_WithDuplicateSide_ReturnsError()
    {
        // Arrange
        var items = new List<CreateOrderItemRequest>
        {
            new(1, 1), // X Burger
            new(4, 2)  // Batata frita com quantidade 2 (múltiplos acompanhamentos)
        };

        // Act
        var errors = _validationService.ValidateOrderItems(items, _menuItems);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateOrderItems_WithSameSandwichTwice_ReturnsError()
    {
        // Arrange
        var items = new List<CreateOrderItemRequest>
        {
            new(1, 1),
            new(1, 1)
        };

        // Act
        var errors = _validationService.ValidateOrderItems(items, _menuItems);

        // Assert
        Assert.NotEmpty(errors);
        Assert.Contains("foi adicionado múltiplas vezes", errors.FirstOrDefault(e => e.Contains("múltiplas")) ?? string.Empty);
    }

    [Fact]
    public void ValidateOrderItems_WithZeroQuantity_ReturnsError()
    {
        // Arrange
        var items = new List<CreateOrderItemRequest>
        {
            new(1, 0)
        };

        // Act
        var errors = _validationService.ValidateOrderItems(items, _menuItems);

        // Assert
        Assert.Single(errors);
        Assert.Contains("maior que zero", errors[0]);
    }

    [Fact]
    public void ValidateOrderItems_WithNegativeQuantity_ReturnsError()
    {
        // Arrange
        var items = new List<CreateOrderItemRequest>
        {
            new(1, -1)
        };

        // Act
        var errors = _validationService.ValidateOrderItems(items, _menuItems);

        // Assert
        Assert.Single(errors);
        Assert.Contains("maior que zero", errors[0]);
    }

    [Fact]
    public void ValidateOrderItems_WithInvalidMenuItemId_ReturnsError()
    {
        // Arrange
        var items = new List<CreateOrderItemRequest>
        {
            new(999, 1)
        };

        // Act
        var errors = _validationService.ValidateOrderItems(items, _menuItems);

        // Assert
        Assert.Empty(errors);
    }
}