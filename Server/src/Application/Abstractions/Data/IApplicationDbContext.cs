using Domain.Banners;
using Domain.Products;
using Domain.Promotions;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Banner> Banners { get; }
    DbSet<Promotion> Promotions { get; }
    DbSet<Address> Addresses { get; }
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    DbSet<Brand> Brands { get; }
    DbSet<Color> Colors { get; }
    DbSet<Size> Sizes { get; }
    DbSet<Subcategory> Subcategories { get; }
    DbSet<Gender> Genders { get; }
    DbSet<ProductPhoto> ProductPhotos { get; }
    DbSet<ProductColor> ProductColors { get; }
    DbSet<ProductGender> ProductGenders { get; }
    DbSet<ProductVariant> ProductVariants { get; }
    DbSet<CategoryGender> CategoryGenders { get; }

    DbSet<CategorySize> CategorySizes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
