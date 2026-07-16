using Application.Abstractions.Messaging;
using Application.Abstractions.Users;
using SharedKernel;

namespace Application.Users.Addresses.SetDefaultAddress;

internal sealed class SetDefaultAddressCommandHandler(
    IUserProfileService userProfileService) : ICommandHandler<SetDefaultAddressCommand>
{
    public Task<Result> Handle(SetDefaultAddressCommand command, CancellationToken cancellationToken)
    {
        return userProfileService.SetDefaultAddressAsync(command.AddressId, cancellationToken);
    }
}
