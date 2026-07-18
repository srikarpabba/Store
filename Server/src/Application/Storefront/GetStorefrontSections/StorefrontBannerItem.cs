namespace Application.Storefront.GetStorefrontSections;

public sealed record StorefrontBannerItem(
    Guid Id,
    string? Title,
    string? Link,
    string? Photo,
    int SortOrder);
