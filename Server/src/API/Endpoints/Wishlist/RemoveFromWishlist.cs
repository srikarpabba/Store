using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Wishlist.RemoveFromWishlist;

namespace API.Endpoints.Wishlist;

internal sealed class RemoveFromWishlist : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{productId:guid}", async (
            Guid productId,
            ICommandHandler<RemoveFromWishlistCommand> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new RemoveFromWishlistCommand(productId), cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .WithName(nameof(RemoveFromWishlist))
        .WithSummary("Removes a product from the signed-in user's wishlist.")
        .WithDescription("Idempotent — removing a product that isn't wishlisted succeeds as a no-op.")
        .Produces(StatusCodes.Status204NoContent);
    }
}
