using Application.Abstractions.Messaging;

namespace Application.Promotions.GetPromotions;

public sealed record GetPromotionsQuery : IQuery<IReadOnlyList<PromotionResponse>>;
