using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Sizes.UpdateSize;
using SharedKernel.Authorization;

namespace API.Endpoints.Sizes;

internal sealed class UpdateSize : IEndpoint
{
    public sealed record Request(string Name);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateSizeCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateSizeCommand(id, request.Name);

            SharedKernel.Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.SizesUpdate)
        .WithRequestValidation<Request>()
        .WithName(nameof(UpdateSize))
        .WithSummary("Updates a size's name.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
