using Application.Abstractions.Messaging;

namespace Application.Banners.GetBanners;

/// <param name="Storefront">Optional filter — when omitted, banners for every storefront are returned.</param>
public sealed record GetBannersQuery(string? Storefront) : IQuery<IReadOnlyList<BannerResponse>>;
