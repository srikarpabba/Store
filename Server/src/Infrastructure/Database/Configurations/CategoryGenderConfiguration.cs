using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class CategoryGenderConfiguration : IEntityTypeConfiguration<CategoryGender>
{
    public void Configure(EntityTypeBuilder<CategoryGender> builder)
    {
        builder.HasKey(cg => new { cg.CategoryId, cg.GenderId });

        builder.Property(cg => cg.PhotoFileName)
            .HasMaxLength(500);

        builder.HasOne(cg => cg.Category)
               .WithMany(c => c.CategoryGenders)
               .HasForeignKey(cg => cg.CategoryId);

        builder.HasOne(cg => cg.Gender)
               .WithMany(g => g.CategoryGenders)
               .HasForeignKey(cg => cg.GenderId);

        builder.HasQueryFilter(x => !x.Category.IsDeleted);
    }
}
