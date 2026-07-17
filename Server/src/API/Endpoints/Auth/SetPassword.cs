using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Auth.SetPassword;
using SharedKernel;

namespace API.Endpoints.Auth;

internal sealed class SetPassword : IEndpoint
{
    public sealed record Request(string NewPassword);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/set-password", async (
            Request request,
            ICommandHandler<SetPasswordCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new SetPasswordCommand(request.NewPassword);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithRequestValidation<Request>()
        .WithName(nameof(SetPassword))
        .WithSummary("Sets a local password for a Google-only account.")
        .WithDescription("For accounts that already have a password, use /auth/change-password instead.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
