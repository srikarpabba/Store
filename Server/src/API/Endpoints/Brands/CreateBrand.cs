using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Brands.CreateBrand;
using SharedKernel.Authorization;

namespace API.Endpoints.Brands;

internal sealed class CreateBrand : IEndpoint
{
    public sealed record Request(string Name, string? Description, bool IsFeatured);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (
            Request request,
            ICommandHandler<CreateBrandCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateBrandCommand(request.Name, request.Description, request.IsFeatured);

            SharedKernel.Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.CreatedAtRoute(nameof(GetBrand), new { id }, id),
                CustomResults.Problem);
        })
        .HasPermission(Permissions.BrandsCreate)
        .WithRequestValidation<Request>()
        .WithName(nameof(CreateBrand))
        .WithSummary("Creates a brand.")
        .WithDescription("A logo is uploaded separately after creation via /brands/{id}/logo.")
        .Produces<Guid>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
