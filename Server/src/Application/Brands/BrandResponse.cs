namespace Application.Brands;

public sealed record BrandResponse(
    Guid Id,
    string Name,
    string? Description,
    string? Logo,
    bool IsFeatured);
