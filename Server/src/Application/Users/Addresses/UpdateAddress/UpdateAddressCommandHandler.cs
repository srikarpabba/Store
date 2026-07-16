using Application.Abstractions.Messaging;
using Application.Abstractions.Users;
using SharedKernel;

namespace Application.Users.Addresses.UpdateAddress;

internal sealed class UpdateAddressCommandHandler(
    IUserProfileService userProfileService) : ICommandHandler<UpdateAddressCommand>
{
    public Task<Result> Handle(UpdateAddressCommand command, CancellationToken cancellationToken)
    {
        return userProfileService.UpdateAddressAsync(command, cancellationToken);
    }
}
