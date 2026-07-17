using System.Linq.Expressions;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Domain.Products;
using Domain.Users;
using Infrastructure.Authorization;
using Infrastructure.DomainEvents;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using SharedKernel;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider)
    : IdentityDbContext<AppUser, AppRole, Guid>(options), IApplicationDbContext
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Color> Colors { get; set; }
    public DbSet<Size> Sizes { get; set; }
    public DbSet<Gender> Genders { get; set; }
    public DbSet<ProductPhoto> ProductPhotos { get; set; }
    public DbSet<ProductColor> ProductColors { get; set; }
    public DbSet<ProductGender> ProductGenders { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<CategoryGender> CategoryGenders { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema(Schemas.Default);

        // Rename Identity tables
        builder.Entity<AppUser>().ToTable("users");
        builder.Entity<AppRole>().ToTable("roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("user_roles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");

        // Apply global query filters
        ApplySoftDeleteFilters(builder);

        // Apply IEntityTypeConfiguration<T>
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();

        List<IDomainEvent> domainEvents = ExtractDomainEvents();

        int result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync(domainEvents, cancellationToken);

        return result;
    }
    private void ApplyAuditInformation()
    {
        DateTime now = dateTimeProvider.UtcNow;
        Guid? userId = userContext.DefaultorNullUserId;

        IEnumerable<EntityEntry<AuditableEntity>> entries = ChangeTracker
        .Entries<AuditableEntity>()
        .Where(e =>
            e.State == EntityState.Added ||
            e.State == EntityState.Modified ||
            e.State == EntityState.Deleted);

        foreach (EntityEntry<AuditableEntity> entry in entries)
        {
            if (entry.State is EntityState.Modified or EntityState.Deleted)
            {
                entry.Property(x => x.CreatedOnUtc).IsModified = false;
                entry.Property(x => x.CreatedBy).IsModified = false;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedOnUtc = now;
                    entry.Entity.ModifiedOnUtc = now;

                    if (userId.HasValue)
                    {
                        entry.Entity.CreatedBy = userId.Value;
                        entry.Entity.ModifiedBy = userId.Value;
                    }
                    break;

                case EntityState.Modified:

                    entry.Entity.ModifiedOnUtc = now;

                    if (userId.HasValue)
                    {
                        entry.Entity.ModifiedBy = userId.Value;
                    }
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;

                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedOnUtc = now;
                    entry.Entity.ModifiedOnUtc = now;

                    if (userId.HasValue)
                    {
                        entry.Entity.ModifiedBy = userId.Value;
                    }
                    break;
            }
        }
    }
    private static void ApplySoftDeleteFilters(ModelBuilder builder)
    {
        foreach (IMutableEntityType entityType in builder.Model.GetEntityTypes())
        {
            if (!typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType) ||
            IsLookupEntity(entityType.ClrType))
            {
                continue;
            }

            ParameterExpression parameter = Expression.Parameter(entityType.ClrType, "e");

            MemberExpression isDeleted = Expression.Property(
                parameter,
                nameof(AuditableEntity.IsDeleted));

            BinaryExpression body = Expression.Equal(
                isDeleted,
                Expression.Constant(false));

            LambdaExpression lambda = Expression.Lambda(body, parameter);

            builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }

    private static bool IsLookupEntity(Type entityType)
    {
        return typeof(BaseLookupEntity).IsAssignableFrom(entityType);
    }

    private async Task PublishDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        await domainEventsDispatcher.DispatchAsync(domainEvents, cancellationToken);
    }

    private List<IDomainEvent> ExtractDomainEvents()
    {
        var domainEvents = ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                List<IDomainEvent> domainEvents = entity.DomainEvents;

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .ToList();
        return domainEvents;
    }
}
