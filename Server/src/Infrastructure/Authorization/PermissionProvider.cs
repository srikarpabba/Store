using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authorization;

internal sealed class PermissionProvider(ApplicationDbContext context)
{
    public async Task<HashSet<string>> GetForUserIdAsync(Guid userId)
    {
        return await context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Join(
                context.RolePermissions,
                ur => ur.RoleId,
                rp => rp.RoleId,
                (_, rp) => rp)
            .Select(rp => rp.Permission.Name)
            .ToHashSetAsync();
    }
}
