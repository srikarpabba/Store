using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Users.ResendEmailConfirmation;
using SharedKernel;

namespace API.Endpoints.Users;

internal sealed class ResendEmailConfirmation : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/me/resend-confirmation", async (
            ICommandHandler<ResendEmailConfirmationCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new ResendEmailConfirmationCommand(), cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithName(nameof(ResendEmailConfirmation))
        .WithSummary("Resends the email confirmation link.")
        .WithDescription("Rate-limited to once per minute per account.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
