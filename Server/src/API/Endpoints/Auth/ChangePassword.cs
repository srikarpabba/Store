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
        .WithRequestValidation<Request>();
    }
}
