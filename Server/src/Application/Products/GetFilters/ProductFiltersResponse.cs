namespace Application.Products.GetFilters;

public sealed record LookupResponse(Guid Id, string Name);

public sealed record ProductFiltersResponse(
    IReadOnlyList<LookupResponse> Brands,
    IReadOnlyList<LookupResponse> Categories,
    IReadOnlyList<LookupResponse> Colors,
    IReadOnlyList<LookupResponse> Sizes,
    IReadOnlyList<LookupResponse> Genders,
    decimal MinPrice,
    decimal MaxPrice);
