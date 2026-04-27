using GoodHamburger.Application.Services;
using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Tests.Application;

public class DiscountCalculationServiceTests
{
    private readonly DiscountCalculationService _discountService = new();

    [Fact]
    public void CalculateDiscount_WithSandwichBatataBeverage_Returns20Percent()
    {
        // Arrange
        var items = new List<OrderItem>
        {
            new() 
            {
                MenuItem = new MenuItem { Name = "X Burger", Type = MenuItemType.Sandwich },
                UnitPrice = 5.00m,
                Quantity = 1
            },
            new() 
            {
                MenuItem = new MenuItem { Name = "Batata frita", Type = MenuItemType.Side },
                UnitPrice = 2.00m,
                Quantity = 1
            },
            new() 
            {
                MenuItem = new MenuItem { Name = "Refrigerante", Type = MenuItemType.Beverage },
                UnitPrice = 2.50m,
                Quantity = 1
            }
        };

        // Act
        var (discountPercentage, description) = _discountService.CalculateDiscount(items);

        // Assert
        Assert.Equal(20, discountPercentage);
        Assert.Equal("Sanduíche + Batata + Refrigerante", description);
    }

    [Fact]
    public void CalculateDiscount_WithSandwichBeverage_Returns15Percent()
    {
        // Arrange
        var items = new List<OrderItem>
        {
            new() {
                MenuItem = new MenuItem { Name = "X Egg", Type = MenuItemType.Sandwich },
                UnitPrice = 4.50m,
                Quantity = 1
            },
            new() {
                MenuItem = new MenuItem { Name = "Refrigerante", Type = MenuItemType.Beverage },
                UnitPrice = 2.50m,
                Quantity = 1
            }
        };

        // Act
        var (discountPercentage, description) = _discountService.CalculateDiscount(items);

        // Assert
        Assert.Equal(15, discountPercentage);
        Assert.Equal("Sanduíche + Refrigerante", description);
    }

    [Fact]
    public void CalculateDiscount_WithSandwichBatata_Returns10Percent()
    {
        // Arrange
        var items = new List<OrderItem>
        {
            new() {
                MenuItem = new MenuItem { Name = "X Bacon", Type = MenuItemType.Sandwich },
                UnitPrice = 7.00m,
                Quantity = 1
            },
            new() {
                MenuItem = new MenuItem { Name = "Batata frita", Type = MenuItemType.Side },
                UnitPrice = 2.00m,
                Quantity = 1
            }
        };

        // Act
        var (discountPercentage, description) = _discountService.CalculateDiscount(items);

        // Assert
        Assert.Equal(10, discountPercentage);
        Assert.Equal("Sanduíche + Batata", description);
    }

    [Fact]
    public void CalculateDiscount_WithOnlySandwich_ReturnsNoDiscount()
    {
        // Arrange
        var items = new List<OrderItem>
        {
            new() {
                MenuItem = new MenuItem { Name = "X Burger", Type = MenuItemType.Sandwich },
                UnitPrice = 5.00m,
                Quantity = 1
            }
        };

        // Act
        var (discountPercentage, description) = _discountService.CalculateDiscount(items);

        // Assert
        Assert.Equal(0, discountPercentage);
        Assert.Equal("Sem desconto", description);
    }

    [Fact]
    public void CalculateDiscount_WithOnlyBatataAndBeverage_ReturnsNoDiscount()
    {
        // Arrange
        var items = new List<OrderItem>
        {
            new() {
                MenuItem = new MenuItem { Name = "Batata frita", Type = MenuItemType.Side },
                UnitPrice = 2.00m,
                Quantity = 1
            },
            new() {
                MenuItem = new MenuItem { Name = "Refrigerante", Type = MenuItemType.Beverage },
                UnitPrice = 2.50m,
                Quantity = 1
            }
        };

        // Act
        var (discountPercentage, description) = _discountService.CalculateDiscount(items);

        // Assert
        Assert.Equal(0, discountPercentage);
        Assert.Equal("Sem desconto", description);
    }
}