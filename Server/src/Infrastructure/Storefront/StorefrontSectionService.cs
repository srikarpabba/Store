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
    private sealed record BannerRow(Guid Id, string? Title, string? LinkUrl, string? ImageFileName, int SortOrder);

    private sealed record CategoryRow(Guid Id, string Name, string? PhotoFileName);

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
