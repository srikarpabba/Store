using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class CategorySizeConfiguration : IEntityTypeConfiguration<CategorySize>
{
    public void Configure(EntityTypeBuilder<CategorySize> builder)
    {
        builder.HasKey(cs => new { cs.CategoryId, cs.SizeId });

        builder.HasOne(cs => cs.Category)
               .WithMany(c => c.CategorySizes)
               .HasForeignKey(cs => cs.CategoryId);

        builder.HasOne(cs => cs.Size)
               .WithMany(s => s.CategorySizes)
               .HasForeignKey(cs => cs.SizeId);

        builder.HasQueryFilter(x => !x.Category.IsDeleted);
    }
}
