using Application.Abstractions.Messaging;

namespace Application.Sizes.UpdateSize;

public sealed record UpdateSizeCommand(Guid Id, string Name) : ICommand;
