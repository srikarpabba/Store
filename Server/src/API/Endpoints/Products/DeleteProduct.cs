using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Products.DeleteProduct;
using SharedKernel.Authorization;

namespace API.Endpoints.Products;

internal sealed class DeleteProduct : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteProductCommand> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(
                new DeleteProductCommand(id),
                cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.ProductsDelete)
        .WithName(nameof(DeleteProduct))
        .WithSummary("Deletes a product and its variants.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
