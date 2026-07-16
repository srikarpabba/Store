using Application.Abstractions.Messaging;
using Application.Abstractions.Users;
using SharedKernel;

namespace Application.Users.Addresses.GetMyAddresses;

internal sealed class GetMyAddressesQueryHandler(
    IUserProfileService userProfileService) : IQueryHandler<GetMyAddressesQuery, IReadOnlyList<AddressResponse>>
{
    public Task<Result<IReadOnlyList<AddressResponse>>> Handle(GetMyAddressesQuery query, CancellationToken cancellationToken)
    {
        return userProfileService.GetAddressesAsync(cancellationToken);
    }
}
