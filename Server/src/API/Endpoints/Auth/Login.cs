using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Auth;
using Application.Auth.Login;

namespace API.Endpoints.Auth;

internal sealed class Login : IEndpoint
{
    public sealed record Request(string Email, string Password);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async (
            Request request,
            ICommandHandler<LoginUserCommand, AccessTokensResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginUserCommand(request.Email, request.Password);

            SharedKernel.Result<AccessTokensResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithRequestValidation<Request>();
    }
}
