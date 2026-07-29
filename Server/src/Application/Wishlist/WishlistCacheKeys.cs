namespace Application.Wishlist;

/// <summary>
/// One cache entry per user (not tagged) — unlike product listings, a
/// wishlist has exactly one key per user, so direct key removal on write is
/// simpler than tag-based invalidation and there's nothing to gain from it.
/// </summary>
internal static class WishlistCacheKeys
{
    public static string ForUser(Guid userId) => $"wishlist:{userId}";
}
