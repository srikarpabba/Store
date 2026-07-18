using Application.Abstractions.Messaging;

namespace Application.Sizes.DeleteSize;

public sealed record DeleteSizeCommand(Guid Id) : ICommand;
