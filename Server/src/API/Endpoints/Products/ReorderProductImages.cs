using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Products.ReorderProductImages;
using SharedKernel.Authorization;

namespace API.Endpoints.Products;

internal sealed class ReorderProductImages : IEndpoint
{
    public sealed record Request(List<Guid> PhotoIds);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{productId:guid}/colors/{productColorId:guid}/images/order", async (
            Guid productId,
            Guid productColorId,
            Request request,
            ICommandHandler<ReorderProductImagesCommand> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(
                new ReorderProductImagesCommand(productId, productColorId, request.PhotoIds),
                cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.ProductsUpdate)
        .WithName(nameof(ReorderProductImages))
        .WithSummary("Sets the display order of a color's photos.")
        .WithDescription("Send every photo id of the color in the desired order. The main photo still always displays first in storefront slideshows.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
