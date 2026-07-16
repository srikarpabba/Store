using Application.Abstractions.Authentication;
using Domain.Users;
using SharedKernel;

namespace Application.Auth.Register;

internal sealed class UserRegisteredDomainEventHandler(
    IIdentityService identityService) : IDomainEventHandler<UserRegisteredDomainEvent>
{
    public async Task Handle(UserRegisteredDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // Sends the "confirm your email" link. Users created via Google
        // arrive already confirmed and are skipped inside the service.
        await identityService.SendEmailConfirmationAsync(domainEvent.UserId, cancellationToken);
    }
}
