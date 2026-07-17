using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Brands.DeleteBrand;
using SharedKernel.Authorization;

namespace API.Endpoints.Brands;

internal sealed class DeleteBrand : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteBrandCommand> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new DeleteBrandCommand(id), cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.BrandsDelete)
        .WithName(nameof(DeleteBrand))
        .WithSummary("Deletes a brand.")
        .WithDescription("Blocked with a conflict if any product still uses this brand.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
