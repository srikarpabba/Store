using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Banners.GetBanners;

internal sealed class GetBannersQueryHandler(
    IApplicationDbContext context,
    IFileStorage fileStorage)
    : IQueryHandler<GetBannersQuery, IReadOnlyList<BannerResponse>>
{
    private sealed record BannerRow(
        Guid Id,
        string Storefront,
        string? Title,
        string? LinkUrl,
        string? ImageFileName,
        int SortOrder,
        bool IsActive);

    public async Task<Result<IReadOnlyList<BannerResponse>>> Handle(
        GetBannersQuery query,
        CancellationToken cancellationToken)
    {
        List<BannerRow> rows = await context.Banners
            .AsNoTracking()
            .Where(b => query.Storefront == null || b.Storefront == query.Storefront)
            .OrderBy(b => b.Storefront)
            .ThenBy(b => b.SortOrder)
            .Select(b => new BannerRow(b.Id, b.Storefront, b.Title, b.LinkUrl, b.ImageFileName, b.SortOrder, b.IsActive))
            .ToListAsync(cancellationToken);

        var items = rows
            .Select(r => new BannerResponse(
                r.Id,
                r.Storefront,
                r.Title,
                r.LinkUrl,
                r.ImageFileName is null ? null : fileStorage.GetUrl(r.ImageFileName).AbsoluteUri,
                r.SortOrder,
                r.IsActive))
            .ToList();

        return items;
    }
}
