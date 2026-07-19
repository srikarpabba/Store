using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class ProductPhotoConfiguration : IEntityTypeConfiguration<ProductPhoto>
{
    public void Configure(EntityTypeBuilder<ProductPhoto> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.IsMain)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(x => x.ProductColor)
            .WithMany(x => x.Photos)
            .HasForeignKey(x => x.ProductColorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.ProductColor.Product.IsDeleted);
    }
}
