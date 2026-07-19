using Application.Abstractions.Messaging;

namespace Application.Products.UpdateProduct;

public sealed record UpdateVariantRequest(
    Guid? Id,
    Guid ColorId,
    Guid SizeId,
    decimal Price,
    int QuantityInStock,
    string SKU);

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    Guid CategoryId,
    Guid? SubcategoryId,
    Guid BrandId,
    IReadOnlyCollection<Guid> GenderIds,
    IReadOnlyCollection<UpdateVariantRequest> Variants)
    : ICommand;
