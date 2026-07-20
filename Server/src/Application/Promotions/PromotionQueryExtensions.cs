using Domain.Promotions;

namespace Application.Promotions;

public static class PromotionQueryExtensions
{
    /// <summary>Promotions that are switched on and within their date window right now.</summary>
    public static IQueryable<Promotion> Active(this IQueryable<Promotion> promotions)
    {
        DateTime now = DateTime.UtcNow;

        return promotions.Where(p => p.IsActive
            && (p.StartsAtUtc == null || p.StartsAtUtc <= now)
            && (p.EndsAtUtc == null || p.EndsAtUtc >= now));
    }
}
