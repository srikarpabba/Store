using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Brands.UpdateBrand;
using SharedKernel.Authorization;

namespace API.Endpoints.Brands;

internal sealed class UpdateBrand : IEndpoint
{
    public sealed record Request(string Name, string? Description, bool IsFeatured);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateBrandCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateBrandCommand(id, request.Name, request.Description, request.IsFeatured);

            SharedKernel.Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.BrandsUpdate)
        .WithRequestValidation<Request>()
        .WithName(nameof(UpdateBrand))
        .WithSummary("Updates a brand's details.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
