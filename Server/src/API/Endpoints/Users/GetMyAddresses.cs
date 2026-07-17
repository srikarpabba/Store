using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Users.Addresses;
using Application.Users.Addresses.GetMyAddresses;

namespace API.Endpoints.Users;

internal sealed class GetMyAddresses : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/me/addresses", async (
            IQueryHandler<GetMyAddressesQuery, IReadOnlyList<AddressResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            SharedKernel.Result<IReadOnlyList<AddressResponse>> result =
                await handler.Handle(new GetMyAddressesQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithName(nameof(GetMyAddresses))
        .WithSummary("Lists the signed-in user's saved addresses.")
        .Produces<IReadOnlyList<AddressResponse>>();
    }
}
