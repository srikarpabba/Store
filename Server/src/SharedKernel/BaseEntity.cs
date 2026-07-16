namespace SharedKernel;

public abstract class BaseEntity : Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
}
