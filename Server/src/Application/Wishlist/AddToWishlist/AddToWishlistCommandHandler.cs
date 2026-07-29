using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using SharedKernel;

namespace Application.Wishlist.AddToWishlist;

internal sealed class AddToWishlistCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    HybridCache cache)
    : ICommandHandler<AddToWishlistCommand>
{
    public async Task<Result> Handle(AddToWishlistCommand command, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        bool productExists = await context.Products
            .AsNoTracking()
            .AnyAsync(p => p.Id == command.ProductId, cancellationToken);

        if (!productExists)
        {
            return Result.Failure(ProductErrors.NotFound(command.ProductId));
        }

        // Idempotent — favoriting an already-wishlisted product is a no-op
        // success, matching a toggle-style heart icon on the client.
        bool alreadyWishlisted = await context.WishlistItems
            .AsNoTracking()
            .AnyAsync(w => w.UserId == userId && w.ProductId == command.ProductId, cancellationToken);

        if (!alreadyWishlisted)
        {
            context.WishlistItems.Add(new WishlistItem
            {
                UserId = userId,
                ProductId = command.ProductId,
                CreatedOnUtc = dateTimeProvider.UtcNow
            });

            await context.SaveChangesAsync(cancellationToken);

            await cache.RemoveAsync(WishlistCacheKeys.ForUser(userId), cancellationToken);
        }

        return Result.Success();
    }
}
