namespace Application.Wishlist;

public sealed record WishlistItemResponse(
    Guid ProductId,
    string ProductName,
    string? Image,
    decimal StartingPrice,
    decimal? DiscountPercentage,
    DateTime? SaleEndsAtUtc,
    DateTime CreatedOnUtc);
