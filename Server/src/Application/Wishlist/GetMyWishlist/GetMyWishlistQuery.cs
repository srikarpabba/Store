using Application.Abstractions.Messaging;

namespace Application.Wishlist.GetMyWishlist;

public sealed record GetMyWishlistQuery : IQuery<IReadOnlyList<WishlistItemResponse>>;
