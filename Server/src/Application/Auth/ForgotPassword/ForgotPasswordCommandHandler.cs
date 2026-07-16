using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Auth.ForgotPassword;

internal sealed class ForgotPasswordCommandHandler(
    IIdentityService identityService) : ICommandHandler<ForgotPasswordCommand>
{
    public Task<Result> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        return identityService.ForgotPasswordAsync(command, cancellationToken);
    }
}
