using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Users.Addresses.DeleteAddress;
using SharedKernel;

namespace API.Endpoints.Users;

internal sealed class DeleteAddress : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/me/addresses/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteAddressCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new DeleteAddressCommand(id), cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        });
    }
}
