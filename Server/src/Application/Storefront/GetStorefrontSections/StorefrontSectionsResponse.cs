namespace Application.Storefront.GetStorefrontSections;

public sealed record StorefrontSectionsResponse(
    IReadOnlyList<StorefrontSectionResponse> Sections);
