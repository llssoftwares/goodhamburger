using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Infrastructure.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.UnitPrice)
            .HasPrecision(10, 2);

        builder.HasOne(e => e.MenuItem)
            .WithMany()
            .HasForeignKey(e => e.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}