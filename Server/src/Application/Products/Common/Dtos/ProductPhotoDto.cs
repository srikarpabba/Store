namespace Application.Products.Common.Dtos;

internal sealed record ProductPhotoDto(
    Guid Id,
    string FileName,
    bool IsMain);
