using SharedKernel;

namespace Infrastructure.Authorization;

public sealed class Permission : AuditableEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}
