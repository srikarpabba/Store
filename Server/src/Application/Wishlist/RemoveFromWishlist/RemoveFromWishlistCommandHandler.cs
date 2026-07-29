using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using SharedKernel;

namespace Application.Wishlist.RemoveFromWishlist;

internal sealed class RemoveFromWishlistCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    HybridCache cache)
    : ICommandHandler<RemoveFromWishlistCommand>
{
    public async Task<Result> Handle(RemoveFromWishlistCommand command, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        // Idempotent — removing a product that isn't wishlisted is a no-op
        // success, matching a toggle-style heart icon on the client.
        WishlistItem? item = await context.WishlistItems
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == command.ProductId, cancellationToken);

        if (item is not null)
        {
            context.WishlistItems.Remove(item);

            await context.SaveChangesAsync(cancellationToken);

            await cache.RemoveAsync(WishlistCacheKeys.ForUser(userId), cancellationToken);
        }

        return Result.Success();
    }
}
