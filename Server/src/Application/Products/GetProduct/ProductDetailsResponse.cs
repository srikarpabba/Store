namespace Application.Products.GetProduct;

public sealed record ProductDetailsResponse(
    Guid Id,
    string Name,
    string Description,
    CategoryResponse Category,
    BrandResponse Brand,
    decimal Rating,
    IReadOnlyList<ProductColorResponse> Colors,
    IReadOnlyList<GenderResponse> Genders,
    IReadOnlyList<ProductVariantResponse> Variants);
