using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Auth.Register;

namespace API.Endpoints.Auth;

internal sealed class Register : IEndpoint
{
    public sealed record Request(string Email, string FirstName, string LastName, string Password);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/register", async (
            Request request,
            ICommandHandler<RegisterUserCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterUserCommand(
                request.Email,
                request.FirstName,
                request.LastName,
                request.Password);

            SharedKernel.Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithRequestValidation<Request>()
        .WithName(nameof(Register))
        .WithSummary("Creates a new customer account.")
        .WithDescription("Returns the new user's id. The email/password sign-in path only — Google sign-up uses /auth/google.")
        .Produces<Guid>()
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
