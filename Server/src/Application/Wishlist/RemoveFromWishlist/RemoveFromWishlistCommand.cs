using Application.Abstractions.Messaging;

namespace Application.Wishlist.RemoveFromWishlist;

public sealed record RemoveFromWishlistCommand(Guid ProductId) : ICommand;
