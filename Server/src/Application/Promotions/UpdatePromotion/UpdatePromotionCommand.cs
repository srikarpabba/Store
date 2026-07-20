using Application.Abstractions.Messaging;

namespace Application.Promotions.UpdatePromotion;

public sealed record UpdatePromotionCommand(
    Guid Id,
    string Name,
    decimal DiscountPercentage,
    DateTime? StartsAtUtc,
    DateTime? EndsAtUtc,
    bool IsActive,
    Guid? ProductId,
    Guid? BrandId)
    : ICommand;
