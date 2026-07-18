using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Colors.CreateColor;
using SharedKernel.Authorization;

namespace API.Endpoints.Colors;

internal sealed class CreateColor : IEndpoint
{
    public sealed record Request(string Name, string HexCode);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (
            Request request,
            ICommandHandler<CreateColorCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateColorCommand(request.Name, request.HexCode);

            SharedKernel.Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.CreatedAtRoute(nameof(GetColor), new { id }, id),
                CustomResults.Problem);
        })
        .HasPermission(Permissions.ColorsCreate)
        .WithRequestValidation<Request>()
        .WithName(nameof(CreateColor))
        .WithSummary("Creates a color.")
        .Produces<Guid>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
