namespace Application.Products.GetFilters;

public sealed record LookupResponse(Guid Id, string Name);

/// <summary>
/// A category lookup carrying the genders it's restricted to. An empty
/// <see cref="GenderIds"/> means the category is unisex — valid for any
/// gender.
/// </summary>
public sealed record CategoryLookupResponse(Guid Id, string Name, IReadOnlyList<Guid> GenderIds);

public sealed record ProductFiltersResponse(
    IReadOnlyList<LookupResponse> Brands,
    IReadOnlyList<CategoryLookupResponse> Categories,
    IReadOnlyList<LookupResponse> Colors,
    IReadOnlyList<LookupResponse> Sizes,
    IReadOnlyList<LookupResponse> Genders,
    decimal MinPrice,
    decimal MaxPrice);
