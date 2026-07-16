namespace Application.Products.GetProduct;

public sealed record ProductColorResponse(
    Guid ProductColorId,
    Guid ColorId,
    string ColorName,
    string HexCode,
    IReadOnlyList<ProductPhotoResponse> Photos);
