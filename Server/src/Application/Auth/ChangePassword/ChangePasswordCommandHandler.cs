using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Auth.ChangePassword;

internal sealed class ChangePasswordCommandHandler(
    IIdentityService identityService) : ICommandHandler<ChangePasswordCommand>
{
    public Task<Result> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        return identityService.ChangePasswordAsync(command, cancellationToken);
    }
}
