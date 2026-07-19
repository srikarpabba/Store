using Application.Products.Common.Responses;

namespace Application.Products.GetProduct;

public sealed record ProductDetailsResponse(
    Guid Id,
    string Name,
    string Description,
    ProductCategoryResponse Category,
    ProductSubcategoryResponse? Subcategory,
    BrandResponse Brand,
    decimal Rating,
    IReadOnlyList<ProductColorResponse> Colors,
    IReadOnlyList<GenderResponse> Genders,
    IReadOnlyList<ProductVariantResponse> Variants);
