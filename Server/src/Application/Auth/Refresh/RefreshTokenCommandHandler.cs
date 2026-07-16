using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Auth.Refresh;

internal sealed class RefreshTokenCommandHandler(
    IIdentityService identityService) : ICommandHandler<RefreshTokenCommand, AccessTokensResponse>
{
    public Task<Result<AccessTokensResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        return identityService.RefreshTokenAsync(command, cancellationToken);
    }
}
