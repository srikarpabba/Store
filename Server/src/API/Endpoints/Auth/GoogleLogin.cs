using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Auth.GoogleLogin;

namespace API.Endpoints.Auth;

internal sealed class GoogleLogin : IEndpoint
{
    public sealed record Request(string IdToken);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/google", async (
            Request request,
            ICommandHandler<GoogleLoginCommand, GoogleAuthResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new GoogleLoginCommand(request.IdToken);

            SharedKernel.Result<GoogleAuthResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithRequestValidation<Request>()
        .WithName(nameof(GoogleLogin))
        .WithSummary("Signs in or registers using a Google ID token.")
        .WithDescription("Creates the account on first sign-in. Returns an access/refresh token pair.")
        .Produces<GoogleAuthResponse>()
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
