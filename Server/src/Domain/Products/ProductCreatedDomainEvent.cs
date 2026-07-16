using SharedKernel;

namespace Domain.Products;

public sealed record ProductCreatedDomainEvent(Guid TodoItemId) : IDomainEvent;
