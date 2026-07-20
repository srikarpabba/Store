using System.Diagnostics;
using Application.Abstractions.Data;
using Application.Abstractions.Storage;
using Application.Abstractions.Storefront;
using Application.Common.Constants;
using Application.Storefront.GetStorefrontSections;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Storefront;

internal sealed class StorefrontSectionService(
    IApplicationDbContext context,
    IFileStorage fileStorage)
    : IStorefrontSectionService
{
    private const int NewArrivalsCount = 4;

    private sealed record BannerRow(Guid Id, string? Title, string? LinkUrl, string? ImageFileName, int SortOrder);

    private sealed record CategoryRow(Guid Id, string Name, string? PhotoFileName);

    private sealed record BrandRow(Guid Id, string Name, string? LogoFileName);

    private sealed record NewArrivalRow(
        Guid Id,
        string Name,
        decimal StartingPrice,
        decimal Rating,
        string? ImageFileName,
        Guid CategoryId,
        string CategoryName,
        Guid? SubcategoryId,
        string? SubcategoryName,
        decimal? DiscountPercentage,
        DateTime? SaleEndsAtUtc,
        List<NewArrivalColorRow> Colors);

    private sealed record NewArrivalColorRow(
        Guid ProductColorId,
        Guid ColorId,
        string ColorName,
        string HexCode,
        List<NewArrivalPhotoRow> Photos);

    private sealed record NewArrivalPhotoRow(Guid Id, string FileName, bool IsMain);

    public async Task<IReadOnlyList<StorefrontSectionResponse>> GetSectionsAsync(string storefront, CancellationToken cancellationToken)
    {
        // the query validator already confirmed this is a recognized storefront
        string normalized = Storefronts.Normalize(storefront)!;

        return normalized switch
        {
            Storefronts.Men => await GetGenderScopedSectionsAsync(normalized, "Male", cancellationToken),
            Storefronts.Women => await GetGenderScopedSectionsAsync(normalized, "Female", cancellationToken),
            // New/Sale are dynamic product listings, not gender-scoped
            // storefronts — only banners apply to them
            Storefronts.New or Storefronts.Sale => await GetBannersOnlySectionsAsync(normalized, cancellationToken),
            _ => throw new UnreachableException()
        };
    }

    private async Task<IReadOnlyList<StorefrontSectionResponse>> GetGenderScopedSectionsAsync(
        string storefront,
        string genderName,
        CancellationToken cancellationToken)
    {
        var sections = new List<StorefrontSectionResponse>();

        IReadOnlyList<StorefrontBannerItem> banners = await GetBannerItemsAsync(storefront, cancellationToken);
        if (banners.Count > 0)
        {
            sections.Add(new StorefrontSectionResponse("banners", "Banners", StorefrontSectionType.Banner, sections.Count, banners));
        }

        IReadOnlyList<StorefrontCategoryItem> categories = await GetCategoryItemsAsync(genderName, cancellationToken);
        if (categories.Count > 0)
        {
            sections.Add(new StorefrontSectionResponse("categories", "Shop by Category", StorefrontSectionType.Category, sections.Count, categories));
        }

        IReadOnlyList<StorefrontProductItem> newArrivals = await GetNewArrivalsAsync(genderName, cancellationToken);
        if (newArrivals.Count > 0)
        {
            sections.Add(new StorefrontSectionResponse("new-arrivals", "New Arrivals", StorefrontSectionType.Product, sections.Count, newArrivals));
        }

        IReadOnlyList<StorefrontBrandItem> featuredBrands = await GetFeaturedBrandsAsync(cancellationToken);
        if (featuredBrands.Count > 0)
        {
            sections.Add(new StorefrontSectionResponse("featured-brands", "Featured Brands", StorefrontSectionType.Brand, sections.Count, featuredBrands));
        }

        return sections;
    }

    private async Task<IReadOnlyList<StorefrontSectionResponse>> GetBannersOnlySectionsAsync(
        string storefront,
        CancellationToken cancellationToken)
    {
        var sections = new List<StorefrontSectionResponse>();

        IReadOnlyList<StorefrontBannerItem> banners = await GetBannerItemsAsync(storefront, cancellationToken);
        if (banners.Count > 0)
        {
            sections.Add(new StorefrontSectionResponse("banners", "Banners", StorefrontSectionType.Banner, sections.Count, banners));
        }

        return sections;
    }

    private async Task<IReadOnlyList<StorefrontBannerItem>> GetBannerItemsAsync(string storefront, CancellationToken cancellationToken)
    {
        List<BannerRow> rows = await context.Banners
            .AsNoTracking()
            .Where(b => b.Storefront == storefront && b.IsActive)
            .OrderBy(b => b.SortOrder)
            .Select(b => new BannerRow(b.Id, b.Title, b.LinkUrl, b.ImageFileName, b.SortOrder))
            .ToListAsync(cancellationToken);

        return rows
            .Select(r => new StorefrontBannerItem(
                r.Id,
                r.Title,
                r.LinkUrl,
                r.ImageFileName is null ? null : fileStorage.GetUrl(r.ImageFileName).AbsoluteUri,
                r.SortOrder))
            .ToList();
    }

    private async Task<IReadOnlyList<StorefrontProductItem>> GetNewArrivalsAsync(string genderName, CancellationToken cancellationToken)
    {
        List<NewArrivalRow> rows = await context.Products
            .AsNoTracking()
            .Where(p => p.Variants.Any() && p.ProductGenders.Any(pg => pg.Gender.Name == genderName))
            .OrderByDescending(p => p.CreatedOnUtc)
            .Take(NewArrivalsCount)
            .Select(p => new NewArrivalRow(
                p.Id,
                p.Name,
                p.Variants.Min(v => v.Price),
                p.Rating,
                p.ProductColors
                    .SelectMany(pc => pc.Photos)
                    .OrderByDescending(ph => ph.IsMain)
                    .ThenBy(ph => ph.SortOrder)
                    .ThenBy(ph => ph.CreatedOnUtc)
                    .Select(ph => ph.FileName)
                    .FirstOrDefault(),
                p.CategoryId,
                p.Category.Name,
                p.SubcategoryId,
                p.Subcategory == null ? null : p.Subcategory.Name,
                // Raw inline LINQ rather than a shared extension method
                // here — EF Core can't translate a custom IQueryable
                // extension call made from this deep inside a nested Select
                // projection.
                context.Promotions
                    .Where(promo => promo.IsActive
                        && (promo.StartsAtUtc == null || promo.StartsAtUtc <= DateTime.UtcNow)
                        && (promo.EndsAtUtc == null || promo.EndsAtUtc >= DateTime.UtcNow)
                        && (promo.ProductId == p.Id || promo.BrandId == p.BrandId))
                    .OrderByDescending(promo => promo.DiscountPercentage)
                    .ThenBy(promo => promo.Id)
                    .Select(promo => (decimal?)promo.DiscountPercentage)
                    .FirstOrDefault(),
                context.Promotions
                    .Where(promo => promo.IsActive
                        && (promo.StartsAtUtc == null || promo.StartsAtUtc <= DateTime.UtcNow)
                        && (promo.EndsAtUtc == null || promo.EndsAtUtc >= DateTime.UtcNow)
                        && (promo.ProductId == p.Id || promo.BrandId == p.BrandId))
                    .OrderByDescending(promo => promo.DiscountPercentage)
                    .ThenBy(promo => promo.Id)
                    .Select(promo => promo.EndsAtUtc)
                    .FirstOrDefault(),
                p.ProductColors
                    .Select(pc => new NewArrivalColorRow(
                        pc.Id,
                        pc.ColorId,
                        pc.Color.Name,
                        pc.Color.HexCode,
                        pc.Photos
                            .OrderByDescending(ph => ph.IsMain)
                            .ThenBy(ph => ph.SortOrder)
                            .ThenBy(ph => ph.CreatedOnUtc)
                            .Select(ph => new NewArrivalPhotoRow(ph.Id, ph.FileName, ph.IsMain))
                            .ToList()))
                    .ToList()))
            .ToListAsync(cancellationToken);

        return rows
            .Select(r => new StorefrontProductItem(
                r.Id,
                r.Name,
                r.StartingPrice,
                r.Rating,
                r.ImageFileName is null ? null : fileStorage.GetUrl(r.ImageFileName).AbsoluteUri,
                new StorefrontProductCategory(r.CategoryId, r.CategoryName),
                r.SubcategoryId is null || r.SubcategoryName is null
                    ? null
                    : new StorefrontProductSubcategory(r.SubcategoryId.Value, r.SubcategoryName),
                r.DiscountPercentage,
                r.SaleEndsAtUtc,
                r.Colors
                    .Select(c => new StorefrontProductColor(
                        c.ProductColorId,
                        c.ColorId,
                        c.ColorName,
                        c.HexCode,
                        c.Photos
                            .Select(ph => new StorefrontProductPhoto(ph.Id, fileStorage.GetUrl(ph.FileName).AbsoluteUri, ph.IsMain))
                            .ToList()))
                    .ToList()))
            .ToList();
    }

    private async Task<IReadOnlyList<StorefrontBrandItem>> GetFeaturedBrandsAsync(CancellationToken cancellationToken)
    {
        // Brands aren't gender-scoped like categories are, so the same
        // featured lineup shows on both /men and /women.
        List<BrandRow> rows = await context.Brands
            .AsNoTracking()
            .Where(b => b.IsFeatured)
            .OrderBy(b => b.Name)
            .Select(b => new BrandRow(b.Id, b.Name, b.LogoFileName))
            .ToListAsync(cancellationToken);

        return rows
            .Select(r => new StorefrontBrandItem(
                r.Id,
                r.Name,
                r.LogoFileName is null ? null : fileStorage.GetUrl(r.LogoFileName).AbsoluteUri))
            .ToList();
    }

    private async Task<IReadOnlyList<StorefrontCategoryItem>> GetCategoryItemsAsync(string genderName, CancellationToken cancellationToken)
    {
        List<CategoryRow> rows = await context.Categories
            .AsNoTracking()
            .Where(c => c.CategoryGenders.Any(cg => cg.Gender.Name == genderName))
            .OrderBy(c => c.CategoryGenders
                .Where(cg => cg.Gender.Name == genderName)
                .Select(cg => cg.SortOrder)
                .FirstOrDefault())
            .ThenBy(c => c.Name)
            .Select(c => new CategoryRow(
                c.Id,
                c.Name,
                c.CategoryGenders.Where(cg => cg.Gender.Name == genderName).Select(cg => cg.PhotoFileName).FirstOrDefault()))
            .ToListAsync(cancellationToken);

        return rows
            .Select(r => new StorefrontCategoryItem(
                r.Id,
                r.Name,
                r.PhotoFileName is null ? null : fileStorage.GetUrl(r.PhotoFileName).AbsoluteUri))
            .ToList();
    }
}
