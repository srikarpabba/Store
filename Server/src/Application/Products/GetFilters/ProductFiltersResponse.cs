namespace Application.Products.GetFilters;

public sealed record LookupResponse(Guid Id, string Name);

/// <summary>
/// A category lookup carrying the genders it's restricted to. An empty
/// <see cref="GenderIds"/> means the category is unisex — valid for any
/// gender. Likewise an empty <see cref="SizeIds"/> means the category is
/// not size-restricted — any size applies.
/// </summary>
public sealed record CategoryLookupResponse(
    Guid Id,
    string Name,
    IReadOnlyList<Guid> GenderIds,
    IReadOnlyList<Guid> SizeIds);

/// <summary>A color lookup carrying its hex, so pickers can render a swatch.</summary>
public sealed record ColorLookupResponse(Guid Id, string Name, string HexCode);

/// <summary>A subcategory lookup carrying its parent category, so pickers can scope by category.</summary>
public sealed record SubcategoryLookupResponse(Guid Id, string Name, Guid CategoryId);

public sealed record ProductFiltersResponse(
    IReadOnlyList<LookupResponse> Brands,
    IReadOnlyList<CategoryLookupResponse> Categories,
    IReadOnlyList<SubcategoryLookupResponse> Subcategories,
    IReadOnlyList<ColorLookupResponse> Colors,
    IReadOnlyList<LookupResponse> Sizes,
    IReadOnlyList<LookupResponse> Genders,
    decimal MinPrice,
    decimal MaxPrice);
