using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Auth.ResetPassword;
using SharedKernel;

namespace API.Endpoints.Auth;

internal sealed class ResetPassword : IEndpoint
{
    public sealed record Request(string Email, string Token, string NewPassword);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/reset-password", async (
            Request request,
            ICommandHandler<ResetPasswordCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ResetPasswordCommand(request.Email, request.Token, request.NewPassword);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithRequestValidation<Request>()
        .WithName(nameof(ResetPassword))
        .WithSummary("Resets a password using the token from the forgot-password email.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
