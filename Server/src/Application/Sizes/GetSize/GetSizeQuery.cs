using Application.Abstractions.Messaging;

namespace Application.Sizes.GetSize;

public sealed record GetSizeQuery(Guid Id) : IQuery<SizeResponse>;
