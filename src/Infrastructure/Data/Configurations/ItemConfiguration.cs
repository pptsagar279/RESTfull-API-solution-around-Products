using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductAPI.Domain.Entities;

namespace ProductAPI.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Item entity
/// </summary>
public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Item");

        builder.HasKey(i => i.ItemId);

        builder.Property(i => i.ItemId)
            .ValueGeneratedOnAdd();

        builder.Property(i => i.ProductId)
            .IsRequired();

        builder.Property(i => i.Quantity)
            .IsRequired();

        // Configure relationship with Product
        builder.HasOne(i => i.Product)
            .WithMany(p => p.Items)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for performance
        builder.HasIndex(i => i.ProductId);
    }
}
