using Application.Abstractions.Messaging;
using Application.Abstractions.Users;
using SharedKernel;

namespace Application.Users.GetProfile;

internal sealed class GetMyProfileQueryHandler(
    IUserProfileService userProfileService) : IQueryHandler<GetMyProfileQuery, ProfileResponse>
{
    public Task<Result<ProfileResponse>> Handle(GetMyProfileQuery query, CancellationToken cancellationToken)
    {
        return userProfileService.GetProfileAsync(cancellationToken);
    }
}
