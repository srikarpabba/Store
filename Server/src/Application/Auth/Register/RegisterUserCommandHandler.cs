using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Auth.Register;

internal sealed class RegisterUserCommandHandler(IIdentityService identityService)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    public Task<Result<Guid>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        return identityService.RegisterAsync(command, cancellationToken);
    }
}
