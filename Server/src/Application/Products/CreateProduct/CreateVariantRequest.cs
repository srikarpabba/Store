namespace Application.Products.CreateProduct;

public sealed record CreateVariantRequest(
        Guid ColorId,
        Guid SizeId,
        decimal Price,
        int QuantityInStock,
        string SKU);
