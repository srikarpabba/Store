using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Auth.ForgotPassword;
using SharedKernel;

namespace API.Endpoints.Auth;

internal sealed class ForgotPassword : IEndpoint
{
    public sealed record Request(string Email);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/forgot-password", async (
            Request request,
            ICommandHandler<ForgotPasswordCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ForgotPasswordCommand(request.Email);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithRequestValidation<Request>()
        .WithName(nameof(ForgotPassword))
        .WithSummary("Sends a password reset link to the given email.")
        .WithDescription("Always succeeds regardless of whether the email exists, to avoid leaking account existence.")
        .Produces(StatusCodes.Status204NoContent);
    }
}
