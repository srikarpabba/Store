using Application.Abstractions.Messaging;

namespace Application.Colors.GetColors;

public sealed record GetColorsQuery : IQuery<IReadOnlyList<ColorResponse>>;
