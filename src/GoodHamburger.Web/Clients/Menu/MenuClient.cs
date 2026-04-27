namespace GoodHamburger.Web.Clients.Menu;

public class MenuClient(HttpClient httpClient, ILogger<MenuClient> logger)
{
    public async Task<List<MenuItemDto>?> GetMenuAsync()
    {
        try
        {
            return await httpClient.GetFromJsonAsync<List<MenuItemDto>>("/api/menu");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching menu");
            throw;
        }
    }
}