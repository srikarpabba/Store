using Application.Abstractions.Messaging;

namespace Application.Promotions.CreatePromotion;

public sealed record CreatePromotionCommand(
    string Name,
    decimal DiscountPercentage,
    DateTime? StartsAtUtc,
    DateTime? EndsAtUtc,
    bool IsActive,
    Guid? ProductId,
    Guid? BrandId)
    : ICommand<Guid>;
