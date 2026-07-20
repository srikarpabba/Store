using Application.Abstractions.Messaging;

namespace Application.Promotions.CreatePromotionBatch;

public sealed record CreatePromotionBatchItem(
    decimal DiscountPercentage,
    DateTime? StartsAtUtc,
    DateTime? EndsAtUtc,
    bool IsActive,
    Guid? ProductId,
    Guid? BrandId);

/// <summary>
/// Creates several promotions at once, all sharing one display <see cref="Name"/>
/// (e.g. "Diwali Sale") so they read as one event in the admin list — each item
/// still carries its own discount, schedule and single-product-or-brand scope,
/// exactly like a promotion created one at a time.
/// </summary>
public sealed record CreatePromotionBatchCommand(
    string Name,
    IReadOnlyList<CreatePromotionBatchItem> Items)
    : ICommand<IReadOnlyList<Guid>>;
