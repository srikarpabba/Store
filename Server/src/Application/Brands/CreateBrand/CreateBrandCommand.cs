using Application.Abstractions.Messaging;

namespace Application.Brands.CreateBrand;

public sealed record CreateBrandCommand(
    string Name,
    string? Description,
    bool IsFeatured)
    : ICommand<Guid>;
