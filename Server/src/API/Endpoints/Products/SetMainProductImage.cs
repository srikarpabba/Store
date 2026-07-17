using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Products.SetMainProductImage;
using SharedKernel.Authorization;

namespace API.Endpoints.Products;

internal sealed class SetMainProductImage : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{productId:guid}/images/{photoId:guid}/main", async (
            Guid productId,
            Guid photoId,
            ICommandHandler<SetMainProductImageCommand> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(
                new SetMainProductImageCommand(productId, photoId),
                cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.ProductsUpdate);
    }
}
