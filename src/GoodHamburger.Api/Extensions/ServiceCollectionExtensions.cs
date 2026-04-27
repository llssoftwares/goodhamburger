using GoodHamburger.Application.Services;
using GoodHamburger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddScoped<OrderService>()
            .AddScoped<MenuService>()
            .AddScoped<OrderValidationService>()
            .AddScoped<DiscountCalculationService>();
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("goodhamburgerdb") 
            ?? configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string not found.");

        services.AddDbContext<GoodHamburgerDbContext>(options => options.UseNpgsql(connectionString));

        return services;
    }
}