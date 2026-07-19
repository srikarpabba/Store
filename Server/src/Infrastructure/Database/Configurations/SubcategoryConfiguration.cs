using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class SubcategoryConfiguration : AuditableBaseEntityConfiguration<Subcategory>
{
    public override void Configure(EntityTypeBuilder<Subcategory> builder)
    {
        base.Configure(builder);

        builder.Property(s => s.Name)
            .HasMaxLength(100)
            .IsRequired();

        // Names only need to be unique within their parent category
        builder.HasIndex(s => new { s.CategoryId, s.Name }).IsUnique();

        builder.HasOne(s => s.Category)
            .WithMany(c => c.Subcategories)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
