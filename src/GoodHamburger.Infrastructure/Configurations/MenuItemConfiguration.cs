using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Infrastructure.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Price)
            .HasPrecision(10, 2);

        builder.Property(e => e.Type)
            .IsRequired();

        builder.HasIndex(e => e.Name)
            .IsUnique();

        builder.HasData(
            new MenuItem { Id = 1, Name = "X Burger", Price = 5.00m, Type = MenuItemType.Sandwich, IsAvailable = true },
            new MenuItem { Id = 2, Name = "X Egg", Price = 4.50m, Type = MenuItemType.Sandwich, IsAvailable = true },
            new MenuItem { Id = 3, Name = "X Bacon", Price = 7.00m, Type = MenuItemType.Sandwich, IsAvailable = true },
            new MenuItem { Id = 4, Name = "Batata frita", Price = 2.00m, Type = MenuItemType.Side, IsAvailable = true },
            new MenuItem { Id = 5, Name = "Refrigerante", Price = 2.50m, Type = MenuItemType.Beverage, IsAvailable = true }
        );
    }
}