namespace Application.Banners;

public sealed record BannerResponse(
    Guid Id,
    string Storefront,
    string? Title,
    string? Link,
    string? Photo,
    int SortOrder,
    bool IsActive);
