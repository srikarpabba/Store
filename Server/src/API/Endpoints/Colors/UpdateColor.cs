using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Colors.UpdateColor;
using SharedKernel.Authorization;

namespace API.Endpoints.Colors;

internal sealed class UpdateColor : IEndpoint
{
    public sealed record Request(string Name, string HexCode);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateColorCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateColorCommand(id, request.Name, request.HexCode);

            SharedKernel.Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.ColorsUpdate)
        .WithRequestValidation<Request>()
        .WithName(nameof(UpdateColor))
        .WithSummary("Updates a color's name and hex.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
