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

    private sealed record NewArrivalRow(
        Guid Id,
        string Name,
        decimal StartingPrice,
        decimal Rating,
        string? ImageFileName,
        Guid CategoryId,
        string CategoryName,
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
            Storefronts.Kids => await GetBannersOnlySectionsAsync(normalized, cancellationToken),
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
                    .ThenBy(ph => ph.CreatedOnUtc)
                    .Select(ph => ph.FileName)
                    .FirstOrDefault(),
                p.CategoryId,
                p.Category.Name,
                p.ProductColors
                    .Select(pc => new NewArrivalColorRow(
                        pc.Id,
                        pc.ColorId,
                        pc.Color.Name,
                        pc.Color.HexCode,
                        pc.Photos
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

    private async Task<IReadOnlyList<StorefrontCategoryItem>> GetCategoryItemsAsync(string genderName, CancellationToken cancellationToken)
    {
        List<CategoryRow> rows = await context.Categories
            .AsNoTracking()
            .Where(c => c.CategoryGenders.Any(cg => cg.Gender.Name == genderName))
            .OrderBy(c => c.Name)
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
