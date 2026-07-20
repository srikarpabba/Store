using SharedKernel;

namespace Domain.Promotions;

public static class PromotionErrors
{
    public static Error NotFound(Guid promotionId) => Error.NotFound(
        "Promotions.NotFound",
        $"The promotion with the Id = '{promotionId}' was not found");

    public static readonly Error InvalidScope = Error.Problem(
        "Promotions.InvalidScope",
        "A promotion must be scoped to exactly one of a product or a brand.");

    public static readonly Error InvalidDateRange = Error.Problem(
        "Promotions.InvalidDateRange",
        "The end date must be after the start date.");
}
