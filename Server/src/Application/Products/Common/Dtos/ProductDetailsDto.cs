namespace Application.Products.Common.Dtos;

internal sealed record ProductDetailsDto(
    Guid Id,
    string Name,
    string Description,
    CategoryDto Category,
    SubcategoryDto? Subcategory,
    BrandDto Brand,
    decimal Rating,
    decimal? DiscountPercentage,
    DateTime? SaleEndsAtUtc,
    IReadOnlyList<ProductColorDto> Colors,
    IReadOnlyList<GenderDto> Genders,
    IReadOnlyList<ProductVariantDto> Variants);
