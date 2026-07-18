using Domain.Banners;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class BannerConfiguration : AuditableBaseEntityConfiguration<Banner>
{
    public override void Configure(EntityTypeBuilder<Banner> builder)
    {
        base.Configure(builder);

        builder.Property(b => b.Storefront)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(b => b.Title)
            .HasMaxLength(200);

        builder.Property(b => b.LinkUrl)
            .HasMaxLength(2048);

        builder.Property(b => b.ImageFileName)
            .HasMaxLength(500);

        builder.HasIndex(b => new { b.Storefront, b.SortOrder });
    }
}
