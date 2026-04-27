using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Domain.ValueObjects;

public record DiscountRule(
    int DiscountPercentage,
    string Description,
    Func<IEnumerable<MenuItemType>, bool> AppliesWhen
);