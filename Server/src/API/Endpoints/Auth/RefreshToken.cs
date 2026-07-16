using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Auth;
using Application.Auth.Refresh;

namespace API.Endpoints.Auth;

internal sealed class RefreshToken : IEndpoint
{
    public sealed record Request(string RefreshToken);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/refresh-token", async (
            Request request,
            ICommandHandler<RefreshTokenCommand, AccessTokensResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RefreshTokenCommand(request.RefreshToken);

            SharedKernel.Result<AccessTokensResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithRequestValidation<Request>();
    }
}
