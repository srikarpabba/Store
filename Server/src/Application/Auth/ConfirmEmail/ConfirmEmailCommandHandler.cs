using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Auth.ConfirmEmail;

internal sealed class ConfirmEmailCommandHandler(
    IIdentityService identityService) : ICommandHandler<ConfirmEmailCommand>
{
    public Task<Result> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        return identityService.ConfirmEmailAsync(command, cancellationToken);
    }
}
