using Application.Products.Common.Responses;

namespace Application.Products.GetProducts;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    decimal StartingPrice,
    decimal Rating,
    string? Image,
    ProductCategoryResponse? Category,
    ProductSubcategoryResponse? Subcategory,
    decimal? DiscountPercentage,
    DateTime? SaleEndsAtUtc,
    IReadOnlyList<ProductColorResponse>? Colors);
