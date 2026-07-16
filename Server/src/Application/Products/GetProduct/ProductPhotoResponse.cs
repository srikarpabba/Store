namespace Application.Products.GetProduct;

public sealed record ProductPhotoResponse(
    Guid Id,
    string FileName,
    bool IsMain);
