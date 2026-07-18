using Application.Abstractions.Messaging;

namespace Application.Colors.DeleteColor;

public sealed record DeleteColorCommand(Guid Id) : ICommand;
