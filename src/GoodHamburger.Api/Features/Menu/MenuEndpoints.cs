using GoodHamburger.Application.Services;

namespace GoodHamburger.Api.Features.Menu;

public static class MenuEndpoints
{
    public static IEndpointRouteBuilder MapMenuEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/menu");

        group.MapGet("/", GetMenu);
        group.MapGet("/{id}", GetMenuItem);

        return app;
    }

    private static async Task<IResult> GetMenu(MenuService menuService)
    {
        var items = await menuService.GetMenuAsync();
        return Results.Ok(items.Select(m => new MenuItemResponse(
            m.Id,
            m.Name,
            m.Price,
            m.Type.ToString()
        )));
    }

    private static async Task<IResult> GetMenuItem(int id, MenuService menuService)
    {
        var item = await menuService.GetMenuItemByIdAsync(id);
        return item is null
            ? Results.NotFound()
            : Results.Ok(new MenuItemResponse(
                item.Id,
                item.Name,
                item.Price,
                item.Type.ToString()
            ));
    }
}