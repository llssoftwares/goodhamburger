using GoodHamburger.Application.Dtos;
using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Application.Services;

public class OrderValidationService
{
    public IList<string> ValidateOrderItems(List<CreateOrderItemRequest> items, IEnumerable<MenuItem> menuItems)
    {
        var errors = new List<string>();

        if (items.Count == 0)
        {
            errors.Add("O pedido deve conter pelo menos um item.");
            return errors;
        }

        var menuItemDict = menuItems.ToDictionary(m => m.Id);

        var duplicateItems = items.GroupBy(i => i.MenuItemId).Where(g => g.Count() > 1);

        foreach (var duplicate in duplicateItems)
        {
            errors.Add($"Item {duplicate.Key} foi adicionado múltiplas vezes. Cada item pode aparecer apenas uma vez no pedido.");
        }

        foreach (var item in items)
        {
            if (item.Quantity <= 0)
            {
                errors.Add($"Quantidade do item {item.MenuItemId} deve ser maior que zero.");
            }
        }

        var sandwichCount = items.Count(i => menuItemDict.TryGetValue(i.MenuItemId, out var m) && m.Type == MenuItemType.Sandwich);
        var sideCount = items.Count(i => menuItemDict.TryGetValue(i.MenuItemId, out var m) && m.Type == MenuItemType.Side);
        var beverageCount = items.Count(i => menuItemDict.TryGetValue(i.MenuItemId, out var m) && m.Type == MenuItemType.Beverage);

        if (sandwichCount > 1)
            errors.Add("O pedido pode conter apenas um sanduíche.");

        if (sideCount > 1)
            errors.Add("O pedido pode conter apenas um acompanhamento.");

        if (beverageCount > 1)
            errors.Add("O pedido pode conter apenas um refrigerante.");

        return errors;
    }
}