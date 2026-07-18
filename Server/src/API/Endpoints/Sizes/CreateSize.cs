using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Sizes.CreateSize;
using SharedKernel.Authorization;

namespace API.Endpoints.Sizes;

internal sealed class CreateSize : IEndpoint
{
    public sealed record Request(string Name);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (
            Request request,
            ICommandHandler<CreateSizeCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateSizeCommand(request.Name);

            SharedKernel.Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.CreatedAtRoute(nameof(GetSize), new { id }, id),
                CustomResults.Problem);
        })
        .HasPermission(Permissions.SizesCreate)
        .WithRequestValidation<Request>()
        .WithName(nameof(CreateSize))
        .WithSummary("Creates a size.")
        .Produces<Guid>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
