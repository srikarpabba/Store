using Application.Abstractions.Messaging;

namespace Application.Products.GetFacets;

public sealed record GetProductFacetsQuery(
    string? Search,
    string[]? Brands,
    string[]? Categories,
    string[]? Subcategories,
    string[]? Colors,
    string[]? Sizes,
    string[]? Genders,
    decimal? MinPrice,
    decimal? MaxPrice)
    : IQuery<ProductFacetsResponse>;
