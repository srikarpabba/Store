using Application.Abstractions.Messaging;
using Application.Common.Pagination;
using Application.Products.Common;

namespace Application.Products.GetProducts;

public sealed record GetProductsQuery(
    string? Search,
    string[]? Brands,
    string[]? Categories,
    string[]? Colors,
    string[]? Sizes,
    string[]? Genders,
    decimal? MinPrice,
    decimal? MaxPrice,
    ProductSort? Sort,
    int? PageIndex,
    int? PageSize
    )
    : IQuery<PagedResponse<ProductResponse>>;
