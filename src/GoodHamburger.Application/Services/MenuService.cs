using GoodHamburger.Domain.Entities;
using GoodHamburger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Application.Services;

public class MenuService(GoodHamburgerDbContext context)
{
    private readonly GoodHamburgerDbContext _context = context;

    public async Task<List<MenuItem>> GetMenuAsync()
    {
        return await _context.MenuItems
            .Where(m => m.IsAvailable)
            .OrderBy(m => m.Type)
            .ThenBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<MenuItem?> GetMenuItemByIdAsync(int id)
    {
        return await _context.MenuItems.FindAsync(id);
    }
}