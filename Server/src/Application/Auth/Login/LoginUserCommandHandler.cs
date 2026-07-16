using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Auth.Login;

internal sealed class LoginUserCommandHandler(
    IIdentityService identityService) : ICommandHandler<LoginUserCommand, AccessTokensResponse>
{
    public Task<Result<AccessTokensResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        return identityService.LoginAsync(command, cancellationToken);
    }
}
