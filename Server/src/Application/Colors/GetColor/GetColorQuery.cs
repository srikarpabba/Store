using Application.Abstractions.Messaging;

namespace Application.Colors.GetColor;

public sealed record GetColorQuery(Guid Id) : IQuery<ColorResponse>;
