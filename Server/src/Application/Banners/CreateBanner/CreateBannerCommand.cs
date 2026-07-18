using Application.Abstractions.Messaging;

namespace Application.Banners.CreateBanner;

public sealed record CreateBannerCommand(
    string Storefront,
    string? Title,
    string? Link,
    int SortOrder,
    bool IsActive)
    : ICommand<Guid>;
