namespace SharedKernel;

public interface IHasDomainEvents
{
    List<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();

    void Raise(IDomainEvent domainEvent);
}
