using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Users.Addresses.UpdateAddress;
using SharedKernel;

namespace API.Endpoints.Users;

internal sealed class UpdateAddress : IEndpoint
{
    public sealed record Request(
        string Line1,
        string? Line2,
        string City,
        string State,
        string PostalCode,
        string Country);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/me/addresses/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateAddressCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateAddressCommand(
                id,
                request.Line1,
                request.Line2,
                request.City,
                request.State,
                request.PostalCode,
                request.Country);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithRequestValidation<Request>()
        .WithName(nameof(UpdateAddress))
        .WithSummary("Updates one of the signed-in user's addresses.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
