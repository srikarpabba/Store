using Application.Abstractions.Messaging;
using Application.Abstractions.Users;
using SharedKernel;

namespace Application.Users.Addresses.AddAddress;

internal sealed class AddAddressCommandHandler(
    IUserProfileService userProfileService) : ICommandHandler<AddAddressCommand, Guid>
{
    public Task<Result<Guid>> Handle(AddAddressCommand command, CancellationToken cancellationToken)
    {
        return userProfileService.AddAddressAsync(command, cancellationToken);
    }
}
