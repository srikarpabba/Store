namespace Application.Storefront.GetStorefrontSections;

public sealed record StorefrontProductItem(
    Guid Id,
    string Name,
    decimal StartingPrice,
    decimal Rating,
    string? Image,
    StorefrontProductCategory? Category,
    StorefrontProductSubcategory? Subcategory,
    IReadOnlyList<StorefrontProductColor> Colors);

public sealed record StorefrontProductCategory(Guid Id, string Name);

public sealed record StorefrontProductSubcategory(Guid Id, string Name);

public sealed record StorefrontProductColor(
    Guid ProductColorId,
    Guid ColorId,
    string ColorName,
    string HexCode,
    IReadOnlyList<StorefrontProductPhoto> Photos);

public sealed record StorefrontProductPhoto(Guid Id, string FileName, bool IsMain);
