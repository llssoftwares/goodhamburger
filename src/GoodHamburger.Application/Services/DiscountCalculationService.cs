using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.ValueObjects;

namespace GoodHamburger.Application.Services;

public class DiscountCalculationService
{
    private readonly List<DiscountRule> _discountRules;

    public DiscountCalculationService()
    {
        _discountRules =
        [
            new(
                20,
                "Sanduíche + Batata + Refrigerante",
                items => HasSandwich(items) && HasSide(items) && HasBeverage(items)
            ),            
            new(
                15,
                "Sanduíche + Refrigerante",
                items => HasSandwich(items) && HasBeverage(items) && !HasSide(items)
            ),            
            new(
                10,
                "Sanduíche + Batata",
                items => HasSandwich(items) && HasSide(items) && !HasBeverage(items)
            )
        ];
    }
    
    public (decimal DiscountPercentage, string Description) CalculateDiscount(IEnumerable<OrderItem> items)
    {
        var itemTypes = items.Select(i => i.MenuItem.Type).ToList();
        
        var applicableRule = _discountRules.FirstOrDefault(rule => rule.AppliesWhen(itemTypes));

        if (applicableRule != null)
        {
            return (applicableRule.DiscountPercentage, applicableRule.Description);
        }

        return (0, "Sem desconto");
    }

    private static bool HasSandwich(IEnumerable<MenuItemType> items) =>
        items.Contains(MenuItemType.Sandwich);

    private static bool HasSide(IEnumerable<MenuItemType> items) =>
        items.Contains(MenuItemType.Side);

    private static bool HasBeverage(IEnumerable<MenuItemType> items) =>
        items.Contains(MenuItemType.Beverage);
}