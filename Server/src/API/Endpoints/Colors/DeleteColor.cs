using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Colors.DeleteColor;
using SharedKernel.Authorization;

namespace API.Endpoints.Colors;

internal sealed class DeleteColor : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteColorCommand> handler,
            CancellationToken cancellationToken) =>
        {
            return (await handler.Handle(new DeleteColorCommand(id), cancellationToken))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.ColorsDelete)
        .WithName(nameof(DeleteColor))
        .WithSummary("Deletes a color.")
        .WithDescription("Blocked with a conflict if any product still uses this color.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
