using Application.Abstractions.Messaging;

namespace Application.Banners.UpdateBanner;

public sealed record UpdateBannerCommand(
    Guid Id,
    string Storefront,
    string? Title,
    string? Link,
    int SortOrder,
    bool IsActive)
    : ICommand;
