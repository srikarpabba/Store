using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Domain.Banners;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Banners.GetBanner;

internal sealed class GetBannerQueryHandler(
    IApplicationDbContext context,
    IFileStorage fileStorage)
    : IQueryHandler<GetBannerQuery, BannerResponse>
{
    public async Task<Result<BannerResponse>> Handle(GetBannerQuery query, CancellationToken cancellationToken)
    {
        Banner? banner = await context.Banners
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == query.Id, cancellationToken);

        if (banner is null)
        {
            return Result.Failure<BannerResponse>(BannerErrors.NotFound(query.Id));
        }

        return new BannerResponse(
            banner.Id,
            banner.Storefront,
            banner.Title,
            banner.LinkUrl,
            banner.ImageFileName is null ? null : fileStorage.GetUrl(banner.ImageFileName).AbsoluteUri,
            banner.SortOrder,
            banner.IsActive);
    }
}
