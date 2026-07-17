using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Users.Addresses.SetDefaultAddress;
using SharedKernel;

namespace API.Endpoints.Users;

internal sealed class SetDefaultAddress : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/me/addresses/{id:guid}/default", async (
            Guid id,
            ICommandHandler<SetDefaultAddressCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new SetDefaultAddressCommand(id), cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithName(nameof(SetDefaultAddress))
        .WithSummary("Marks one of the signed-in user's addresses as the default.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
