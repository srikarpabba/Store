using Application.Abstractions.Messaging;

namespace Application.Colors.UpdateColor;

public sealed record UpdateColorCommand(Guid Id, string Name, string HexCode) : ICommand;
