using Application.Abstractions.Messaging;

namespace Application.Products.SetMainProductImage;

public sealed record SetMainProductImageCommand(Guid ProductId, Guid PhotoId) : ICommand;
