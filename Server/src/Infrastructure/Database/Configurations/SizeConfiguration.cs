using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class SizeConfiguration : AuditableBaseEntityConfiguration<Size>
{
    public override void Configure(EntityTypeBuilder<Size> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(10) // e.g., "XXL", "UK 10"
            .IsRequired();

        builder.HasIndex(x => x.Name).IsUnique();
    }
}
