using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel;

namespace Infrastructure.Database.Configurations;

public class AuditableBaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : AuditableEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Audit fields
        builder.Property(e => e.CreatedOnUtc).IsRequired();
        builder.Property(e => e.ModifiedOnUtc);
        builder.Property(e => e.DeletedOnUtc);

        // Soft delete
        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes for performance
        builder.HasIndex(e => e.CreatedOnUtc).HasDatabaseName($"IX_{typeof(TEntity).Name}_CreatedOn");
        builder.HasIndex(e => e.CreatedBy).HasDatabaseName($"IX_{typeof(TEntity).Name}_CreatedById");
        builder.HasIndex(e => e.IsDeleted).HasDatabaseName($"IX_{typeof(TEntity).Name}_IsDeleted");

        // Composite index for common queries
        builder.HasIndex(e => new { e.IsDeleted, e.CreatedOnUtc }).HasDatabaseName($"IX_{typeof(TEntity).Name}_IsDeleted_CreatedOn");
    }
}
