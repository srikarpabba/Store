using Application.Abstractions.Messaging;

namespace Application.Wishlist.AddToWishlist;

public sealed record AddToWishlistCommand(Guid ProductId) : ICommand;
