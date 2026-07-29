using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Wishlist;
using Application.Wishlist.GetMyWishlist;

namespace API.Endpoints.Wishlist;

internal sealed class GetMyWishlist : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
            IQueryHandler<GetMyWishlistQuery, IReadOnlyList<WishlistItemResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            SharedKernel.Result<IReadOnlyList<WishlistItemResponse>> result =
                await handler.Handle(new GetMyWishlistQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithName(nameof(GetMyWishlist))
        .WithSummary("Lists the signed-in user's wishlist.")
        .Produces<IReadOnlyList<WishlistItemResponse>>();
    }
}
