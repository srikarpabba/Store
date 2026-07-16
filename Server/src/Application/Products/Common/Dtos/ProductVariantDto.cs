namespace Application.Products.Common.Dtos;


internal sealed record ProductVariantDto(
    Guid Id,
    Guid ProductColorId,
    Guid SizeId,
    string Size,
    decimal Price,
    int QuantityInStock,
    string SKU);
