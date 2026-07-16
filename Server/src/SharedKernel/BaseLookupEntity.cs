namespace SharedKernel;

public abstract class BaseLookupEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
}
