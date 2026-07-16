namespace Application.Products.Common.Dtos;

internal sealed record ProductColorDto(
    Guid Id,
    Guid ColorId,
    string Name,
    string HexCode,
    IReadOnlyList<ProductPhotoDto> Photos);
