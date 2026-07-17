using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Users.Addresses.AddAddress;

namespace API.Endpoints.Users;

internal sealed class AddAddress : IEndpoint
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
        app.MapPost("/me/addresses", async (
            Request request,
            ICommandHandler<AddAddressCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AddAddressCommand(
                request.Line1,
                request.Line2,
                request.City,
                request.State,
                request.PostalCode,
                request.Country);

            SharedKernel.Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithRequestValidation<Request>()
        .WithName(nameof(AddAddress))
        .WithSummary("Adds a delivery address for the signed-in user.")
        .WithDescription("The first address added for an account becomes its default.")
        .Produces<Guid>()
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
