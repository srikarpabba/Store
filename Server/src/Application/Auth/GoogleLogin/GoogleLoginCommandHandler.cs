using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Auth.GoogleLogin;

internal sealed class GoogleLoginCommandHandler(
    IIdentityService identityService) : ICommandHandler<GoogleLoginCommand, GoogleAuthResponse>
{
    public Task<Result<GoogleAuthResponse>> Handle(GoogleLoginCommand command, CancellationToken cancellationToken)
    {
        return identityService.LoginWithGoogleAsync(command, cancellationToken);
    }
}
