using Domain.Products;

namespace Domain.Users;

/// <summary>
/// A product a user has favorited — whole-product, not a specific color or
/// size. No soft delete and no audit trail: removing an item is a normal,
/// permanent action the user takes themselves, not something anything else
/// depends on.
/// </summary>
public sealed class WishlistItem
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; } = null!;
    public DateTime CreatedOnUtc { get; set; }
}
