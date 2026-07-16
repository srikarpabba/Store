using SharedKernel;

namespace Domain.Users;

public sealed class Address : AuditableEntity
{
    public required string Line1 { get; set; }
    public string? Line2 { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
    public required string Country { get; set; } = "India";
    public Guid UserId { get; set; }
    public bool IsDefault { get; set; }
}
