using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Auth.ConfirmEmail;
using SharedKernel;

namespace API.Endpoints.Auth;

internal sealed class ConfirmEmail : IEndpoint
{
    public sealed record Request(string Email, string Token);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/confirm-email", async (
            Request request,
            ICommandHandler<ConfirmEmailCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ConfirmEmailCommand(request.Email, request.Token);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithRequestValidation<Request>();
    }
}
