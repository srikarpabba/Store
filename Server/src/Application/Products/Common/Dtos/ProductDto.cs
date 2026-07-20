namespace Application.Products.Common.Dtos;

internal sealed record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal StartingPrice,
    decimal Rating,
    string? Image,
    CategoryDto? Category,
    SubcategoryDto? Subcategory,
    decimal? DiscountPercentage,
    DateTime? SaleEndsAtUtc,
    IReadOnlyList<ProductColorDto>? Colors);
