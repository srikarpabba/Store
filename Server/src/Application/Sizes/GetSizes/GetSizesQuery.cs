using Application.Abstractions.Messaging;

namespace Application.Sizes.GetSizes;

public sealed record GetSizesQuery : IQuery<IReadOnlyList<SizeResponse>>;
