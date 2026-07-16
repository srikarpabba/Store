using Domain.Users;
using Microsoft.AspNetCore.Identity;
using SharedKernel;

namespace Infrastructure.Identity;

public sealed class AppUser : IdentityUser<Guid>, IHasDomainEvents
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public ICollection<Address> Addresses { get; private set; } = [];
    public string? FileName { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastActive { get; set; }
    public DateTime? LastConfirmationEmailSent { get; set; }
    public bool EnableNotifications { get; set; }

    private readonly List<IDomainEvent> _domainEvents = [];
    public List<IDomainEvent> DomainEvents => [.. _domainEvents];
    public void ClearDomainEvents() => _domainEvents.Clear();
    public void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
