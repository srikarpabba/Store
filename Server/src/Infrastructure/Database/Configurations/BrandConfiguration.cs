using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class BrandConfiguration : AuditableBaseEntityConfiguration<Brand>
{
    public override void Configure(EntityTypeBuilder<Brand> builder)
    {
        base.Configure(builder); // Applies audit indices

        builder.Property(b => b.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(b => b.Name).IsUnique();

        // One Brand -> Many Products
        builder.HasMany(b => b.Products)
            .WithOne(p => p.Brand)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
