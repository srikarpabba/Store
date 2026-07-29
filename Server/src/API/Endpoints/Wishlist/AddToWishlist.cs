using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Wishlist.AddToWishlist;

namespace API.Endpoints.Wishlist;

internal sealed class AddToWishlist : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/{productId:guid}", async (
            Guid productId,
            ICommandHandler<AddToWishlistCommand> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new AddToWishlistCommand(productId), cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .WithName(nameof(AddToWishlist))
        .WithSummary("Adds a product to the signed-in user's wishlist.")
        .WithDescription("Idempotent — adding an already-wishlisted product succeeds without duplicating it.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
