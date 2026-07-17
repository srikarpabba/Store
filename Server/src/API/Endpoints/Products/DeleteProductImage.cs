using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Products.DeleteProductImage;
using SharedKernel.Authorization;

namespace API.Endpoints.Products;

internal sealed class DeleteProductImage : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{productId:guid}/images/{photoId:guid}", async (
            Guid productId,
            Guid photoId,
            ICommandHandler<DeleteProductImageCommand> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(
                new DeleteProductImageCommand(productId, photoId),
                cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.ProductsUpdate);
    }
}
