using Application.Abstractions.Messaging;

namespace Application.Colors.CreateColor;

public sealed record CreateColorCommand(string Name, string HexCode) : ICommand<Guid>;
