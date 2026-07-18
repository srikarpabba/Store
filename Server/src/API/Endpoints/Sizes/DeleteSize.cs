using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Sizes.DeleteSize;
using SharedKernel.Authorization;

namespace API.Endpoints.Sizes;

internal sealed class DeleteSize : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteSizeCommand> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new DeleteSizeCommand(id), cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.SizesDelete)
        .WithName(nameof(DeleteSize))
        .WithSummary("Deletes a size.")
        .WithDescription("Blocked with a conflict if any product variant still uses this size.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
