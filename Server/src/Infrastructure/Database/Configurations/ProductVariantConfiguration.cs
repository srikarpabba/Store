using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.SKU)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.SKU)
            .IsUnique();

        builder.Property(x => x.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.QuantityInStock)
            .IsRequired();

        builder.HasOne(x => x.ProductColor)
            .WithMany(x => x.Variants)
            .HasForeignKey(x => x.ProductColorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Size)
            .WithMany(x => x.ProductVariants)
            .HasForeignKey(x => x.SizeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.ProductColorId,
            x.SizeId
        }).IsUnique();

        builder.HasQueryFilter(x => !x.ProductColor.Product.IsDeleted);
    }
}
