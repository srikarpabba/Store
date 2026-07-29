using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
{
    public void Configure(EntityTypeBuilder<WishlistItem> builder)
    {
        builder.HasKey(w => new { w.UserId, w.ProductId });

        builder.HasOne(w => w.Product)
            .WithMany()
            .HasForeignKey(w => w.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(w => w.CreatedOnUtc)
            .IsRequired();

        builder.HasIndex(w => w.UserId);

        // WishlistItem isn't an AuditableEntity (no soft delete of its own), so unlike every
        // other required relationship in the model — which pairs two AuditableEntity sides and
        // gets matching filters automatically — this one needs its filter defined explicitly to
        // mirror Product's. Without it, EF warns that the required side can be filtered out from
        // under the dependent; this filter makes that already-intended behavior (a wishlist item
        // quietly drops off once its product is discontinued) explicit instead of incidental.
        builder.HasQueryFilter(w => !w.Product.IsDeleted);
    }
}
