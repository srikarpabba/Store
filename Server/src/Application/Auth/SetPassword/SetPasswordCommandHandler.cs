using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Auth.SetPassword;

internal sealed class SetPasswordCommandHandler(
    IIdentityService identityService) : ICommandHandler<SetPasswordCommand>
{
    public Task<Result> Handle(SetPasswordCommand command, CancellationToken cancellationToken)
    {
        return identityService.SetPasswordAsync(command, cancellationToken);
    }
}
