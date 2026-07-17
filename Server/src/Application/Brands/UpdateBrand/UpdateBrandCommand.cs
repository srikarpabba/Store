using Application.Abstractions.Messaging;

namespace Application.Brands.UpdateBrand;

public sealed record UpdateBrandCommand(
    Guid Id,
    string Name,
    string? Description,
    bool IsFeatured)
    : ICommand;
