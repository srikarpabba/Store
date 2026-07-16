using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasIndex(x => x.UserId);

        builder.HasIndex(a => new { a.UserId, a.IsDefault })
            .HasFilter("is_default = true")
            .IsUnique();

        builder.Property(a => a.Line1)
            .HasMaxLength(250).
            IsRequired();

        builder.Property(a => a.Line2)
            .HasMaxLength(250);

        builder.Property(a => a.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.State)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.PostalCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.Country)
            .HasMaxLength(100)
            .IsRequired();
    }
}
