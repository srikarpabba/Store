using Application.Abstractions.Messaging;

namespace Application.Products.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string Description,
    Guid CategoryId,
    Guid BrandId,
    IReadOnlyCollection<Guid> GenderIds,
    IReadOnlyCollection<CreateVariantRequest> Variants)
    : ICommand<Guid>;
