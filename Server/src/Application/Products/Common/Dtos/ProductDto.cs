namespace Application.Products.Common.Dtos;

internal sealed record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal StartingPrice,
    decimal Rating,
    string? Image);
