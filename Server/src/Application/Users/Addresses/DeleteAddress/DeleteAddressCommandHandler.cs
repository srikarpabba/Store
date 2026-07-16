using Application.Abstractions.Messaging;
using Application.Abstractions.Users;
using SharedKernel;

namespace Application.Users.Addresses.DeleteAddress;

internal sealed class DeleteAddressCommandHandler(
    IUserProfileService userProfileService) : ICommandHandler<DeleteAddressCommand>
{
    public Task<Result> Handle(DeleteAddressCommand command, CancellationToken cancellationToken)
    {
        return userProfileService.DeleteAddressAsync(command.AddressId, cancellationToken);
    }
}
