using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GoodHamburger.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<GoodHamburgerDbContext>
{
    public GoodHamburgerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GoodHamburgerDbContext>();

        optionsBuilder.UseNpgsql("Host=localhost;Database=design_time;Username=postgres;Password=postgres");

        return new GoodHamburgerDbContext(optionsBuilder.Options);
    }
}