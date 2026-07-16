using Application.Abstractions.Messaging;
using Application.Abstractions.Users;
using SharedKernel;

namespace Application.Users.UpdateProfile;

internal sealed class UpdateMyProfileCommandHandler(
    IUserProfileService userProfileService) : ICommandHandler<UpdateMyProfileCommand>
{
    public Task<Result> Handle(UpdateMyProfileCommand command, CancellationToken cancellationToken)
    {
        return userProfileService.UpdateProfileAsync(command, cancellationToken);
    }
}
