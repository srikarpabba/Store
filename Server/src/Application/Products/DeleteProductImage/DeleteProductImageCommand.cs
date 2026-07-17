using Application.Abstractions.Messaging;

namespace Application.Products.DeleteProductImage;

public sealed record DeleteProductImageCommand(Guid ProductId, Guid PhotoId) : ICommand;
