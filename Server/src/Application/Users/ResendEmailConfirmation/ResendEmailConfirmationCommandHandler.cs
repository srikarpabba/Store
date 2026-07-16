using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Users.ResendEmailConfirmation;

internal sealed class ResendEmailConfirmationCommandHandler(
    IIdentityService identityService,
    IUserContext userContext) : ICommandHandler<ResendEmailConfirmationCommand>
{
    public Task<Result> Handle(ResendEmailConfirmationCommand command, CancellationToken cancellationToken)
    {
        return identityService.SendEmailConfirmationAsync(userContext.UserId, cancellationToken);
    }
}
