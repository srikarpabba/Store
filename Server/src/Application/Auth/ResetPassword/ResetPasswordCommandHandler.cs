using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Auth.ResetPassword;

internal sealed class ResetPasswordCommandHandler(
    IIdentityService identityService) : ICommandHandler<ResetPasswordCommand>
{
    public Task<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        return identityService.ResetPasswordAsync(command, cancellationToken);
    }
}
