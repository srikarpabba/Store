using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Auth.ChangePassword;
using SharedKernel;

namespace API.Endpoints.Auth;

internal sealed class ChangePassword : IEndpoint
{
    public sealed record Request(string CurrentPassword, string NewPassword);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/change-password", async (
            Request request,
            ICommandHandler<ChangePasswordCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ChangePasswordCommand(request.CurrentPassword, request.NewPassword);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithRequestValidation<Request>()
        .WithName(nameof(ChangePassword))
        .WithSummary("Changes the signed-in user's password.")
        .WithDescription("Requires the current password. For accounts with no password yet (Google sign-up), use /auth/set-password instead.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
