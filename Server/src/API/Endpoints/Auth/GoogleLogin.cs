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
        .WithRequestValidation<Request>();
    }
}
