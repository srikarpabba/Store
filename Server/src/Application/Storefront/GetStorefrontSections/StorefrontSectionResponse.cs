namespace Application.Storefront.GetStorefrontSections;

public sealed record StorefrontSectionResponse(
    string Key,
    string Title,
    StorefrontSectionType Type,
    int DisplayOrder,
    object Items);
