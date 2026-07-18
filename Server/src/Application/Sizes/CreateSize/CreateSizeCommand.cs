using Application.Abstractions.Messaging;

namespace Application.Sizes.CreateSize;

public sealed record CreateSizeCommand(string Name) : ICommand<Guid>;
