namespace Application.Promotions;

public sealed record PromotionResponse(
    Guid Id,
    string Name,
    decimal DiscountPercentage,
    DateTime? StartsAtUtc,
    DateTime? EndsAtUtc,
    bool IsActive,
    Guid? ProductId,
    string? ProductName,
    Guid? BrandId,
    string? BrandName);
