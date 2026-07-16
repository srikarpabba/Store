using Application.Abstractions.Messaging;

namespace Application.Products.GetProduct;

public sealed record GetProductQuery(Guid ProductId) : IQuery<ProductDetailsResponse>;
