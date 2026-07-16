using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token)
            .HasMaxLength(512)
            .IsRequired();

        builder.HasIndex(x => x.Token)
            .IsUnique();

        builder.Property(x => x.DeviceName)
            .HasMaxLength(200);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(45);

        builder.Property(x => x.ExpiresOnUtc)
            .IsRequired();

        builder.Property(x => x.RevokedOnUtc);

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.ExpiresOnUtc);

        builder.Ignore(x => x.IsRevoked);
    }
}
