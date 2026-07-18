namespace Application.Storefront.GetStorefrontSections;

public sealed record StorefrontCategoryItem(
    Guid Id,
    string Name,
    string? Photo);
