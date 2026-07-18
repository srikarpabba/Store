using Application.Abstractions.Messaging;

namespace Application.Banners.GetBanner;

public sealed record GetBannerQuery(Guid Id) : IQuery<BannerResponse>;
