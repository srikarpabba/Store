using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class ProductGenderConfiguration : IEntityTypeConfiguration<ProductGender>
{
    public void Configure(EntityTypeBuilder<ProductGender> builder)
    {
        builder.HasKey(ps => new { ps.ProductId, ps.GenderId });

        builder.HasOne(ps => ps.Product)
               .WithMany(p => p.ProductGenders)
               .HasForeignKey(ps => ps.ProductId);

        builder.HasOne(ps => ps.Gender)
               .WithMany(g => g.ProductGenders)
               .HasForeignKey(ps => ps.GenderId);

        builder.HasQueryFilter(x => !x.Product.IsDeleted);
    }
}
