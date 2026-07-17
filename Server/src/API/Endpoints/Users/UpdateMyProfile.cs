using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Users.UpdateProfile;
using SharedKernel;

namespace API.Endpoints.Users;

internal sealed class UpdateMyProfile : IEndpoint
{
    public sealed record Request(string FirstName, string LastName, string Email, string? PhoneNumber);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/me", async (
            Request request,
            ICommandHandler<UpdateMyProfileCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateMyProfileCommand(
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithRequestValidation<Request>()
        .WithName(nameof(UpdateMyProfile))
        .WithSummary("Updates the signed-in user's profile.")
        .WithDescription("Changing the email address resets email confirmation and sends a new confirmation link.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
