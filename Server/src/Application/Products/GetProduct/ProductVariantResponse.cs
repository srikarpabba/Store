namespace Application.Products.GetProduct;

public sealed record ProductVariantResponse(
    Guid Id,
    Guid ProductColorId,
    Guid SizeId,
    string SizeName,
    decimal Price,
    int QuantityInStock,
    string SKU);
