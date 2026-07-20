using Application.Abstractions.Messaging;

namespace Application.Promotions.GetPromotion;

public sealed record GetPromotionQuery(Guid Id) : IQuery<PromotionResponse>;
