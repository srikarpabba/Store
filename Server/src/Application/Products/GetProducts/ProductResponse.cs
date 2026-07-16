namespace Application.Products.GetProducts;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    decimal StartingPrice,
    decimal Rating,
    string? Image);
