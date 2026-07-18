namespace Application.Products.Common.Responses;

public sealed record ProductPhotoResponse(
    Guid Id,
    string FileName,
    bool IsMain);
