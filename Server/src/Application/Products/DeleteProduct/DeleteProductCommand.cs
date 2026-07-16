using Application.Abstractions.Messaging;

namespace Application.Products.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id)
    : ICommand;
