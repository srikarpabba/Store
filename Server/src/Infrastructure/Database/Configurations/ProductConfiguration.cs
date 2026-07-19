using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class ProductConfiguration : AuditableBaseEntityConfiguration<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(2000).IsRequired();

        builder.Property(p => p.Rating)
            .HasColumnType("decimal(3,2)")
            .HasDefaultValue(0m);

        // Indexes
        builder.HasIndex(p => p.Name)
            .HasDatabaseName("IX_Products_Name");

        builder.HasIndex(p => p.BrandId)
            .HasDatabaseName("IX_Products_BrandId");

        builder.HasIndex(p => p.CategoryId)
            .HasDatabaseName("IX_Products_CategoryId");

        builder.HasIndex(p => p.SubcategoryId)
            .HasDatabaseName("IX_Products_SubcategoryId");

        builder.HasOne(p => p.Subcategory)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SubcategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.Rating)
            .HasDatabaseName("IX_Products_Rating");  //For sorting
    }
}
