using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Brands.DeleteBrandLogo;
using SharedKernel.Authorization;

namespace API.Endpoints.Brands;

internal sealed class DeleteBrandLogo : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:guid}/logo", async (
            Guid id,
            ICommandHandler<DeleteBrandLogoCommand> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new DeleteBrandLogoCommand(id), cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.BrandsUpdate)
        .WithName(nameof(DeleteBrandLogo))
        .WithSummary("Deletes a brand's logo.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
