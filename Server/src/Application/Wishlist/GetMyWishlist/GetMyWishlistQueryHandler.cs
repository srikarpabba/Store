using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using SharedKernel;

namespace Application.Wishlist.GetMyWishlist;

internal sealed class GetMyWishlistQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IFileStorage fileStorage,
    HybridCache cache)
    : IQueryHandler<GetMyWishlistQuery, IReadOnlyList<WishlistItemResponse>>
{
    private sealed record WishlistRow(
        Guid ProductId,
        string ProductName,
        decimal StartingPrice,
        string? ImageFileName,
        decimal? DiscountPercentage,
        DateTime? SaleEndsAtUtc,
        DateTime CreatedOnUtc);

    public async Task<Result<IReadOnlyList<WishlistItemResponse>>> Handle(
        GetMyWishlistQuery query,
        CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        // Presigned image URLs (1hr TTL) get baked into the cached response,
        // so this relies on the registered cache duration (5 min default)
        // staying well under that, or they'd be served stale-but-expired.
        IReadOnlyList<WishlistItemResponse> items = await cache.GetOrCreateAsync(
            WishlistCacheKeys.ForUser(userId),
            (Handler: this, UserId: userId),
            static async (state, ct) => await state.Handler.LoadAsync(state.UserId, ct),
            cancellationToken: cancellationToken);

        return Result.Success(items);
    }

    private async Task<IReadOnlyList<WishlistItemResponse>> LoadAsync(Guid userId, CancellationToken cancellationToken)
    {
        List<WishlistRow> rows = await context.WishlistItems
            .AsNoTracking()
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.CreatedOnUtc)
            .Select(w => new WishlistRow(
                w.ProductId,
                w.Product.Name,
                w.Product.Variants.Min(v => v.Price),
                w.Product.ProductColors
                    .SelectMany(pc => pc.Photos)
                    .OrderByDescending(p => p.IsMain)
                    .ThenBy(p => p.SortOrder)
                    .ThenBy(p => p.CreatedOnUtc)
                    .Select(p => p.FileName)
                    .FirstOrDefault(),
                // Raw inline LINQ rather than a shared extension method —
                // EF Core can't translate a custom IQueryable extension
                // call made from this deep inside a nested projection.
                context.Promotions
                    .Where(promo => promo.IsActive
                        && (promo.StartsAtUtc == null || promo.StartsAtUtc <= DateTime.UtcNow)
                        && (promo.EndsAtUtc == null || promo.EndsAtUtc >= DateTime.UtcNow)
                        && (promo.ProductId == w.ProductId || promo.BrandId == w.Product.BrandId))
                    .OrderByDescending(promo => promo.DiscountPercentage)
                    .ThenBy(promo => promo.Id)
                    .Select(promo => (decimal?)promo.DiscountPercentage)
                    .FirstOrDefault(),
                context.Promotions
                    .Where(promo => promo.IsActive
                        && (promo.StartsAtUtc == null || promo.StartsAtUtc <= DateTime.UtcNow)
                        && (promo.EndsAtUtc == null || promo.EndsAtUtc >= DateTime.UtcNow)
                        && (promo.ProductId == w.ProductId || promo.BrandId == w.Product.BrandId))
                    .OrderByDescending(promo => promo.DiscountPercentage)
                    .ThenBy(promo => promo.Id)
                    .Select(promo => promo.EndsAtUtc)
                    .FirstOrDefault(),
                w.CreatedOnUtc))
            .ToListAsync(cancellationToken);

        return rows
            .Select(r => new WishlistItemResponse(
                r.ProductId,
                r.ProductName,
                r.ImageFileName is null ? null : fileStorage.GetUrl(r.ImageFileName).AbsoluteUri,
                r.StartingPrice,
                r.DiscountPercentage,
                r.SaleEndsAtUtc,
                r.CreatedOnUtc))
            .ToList();
    }
}
