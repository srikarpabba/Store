using System.Security.Authentication;
using System.Security.Claims;
using Domain.Users;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authentication;

internal static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        string? userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userId, out Guid parsedUserId) ?
            parsedUserId :
            throw new ApplicationException("User id is unavailable");
    }

    public static async Task<AppUser> GetUserByEmailAsync(this UserManager<AppUser> userManager, ClaimsPrincipal claimsPrincipal)
    {
        return await userManager.Users.FirstOrDefaultAsync(x =>
            x.Email == claimsPrincipal.GetEmail()) ?? throw new AuthenticationException("Invalid authentication.");
    }

    public static async Task<AppUser> GetUserByEmailWithAddressAsync(this UserManager<AppUser> userManager, ClaimsPrincipal claimsPrincipal)
    {
        return await userManager.Users
            .Include(x => x.Addresses)
            .FirstOrDefaultAsync(x => x.Email == claimsPrincipal.GetEmail()) ?? throw new AuthenticationException("Invalid authentication.");
    }

    public static async Task<Address?> GetDefaultAddressAsync(this UserManager<AppUser> userManager, ClaimsPrincipal claimsPrincipal)
    {
        return await userManager.Users
            .Where(x => x.Email == claimsPrincipal.GetEmail())
            .SelectMany(x => x.Addresses)
            .FirstOrDefaultAsync(a => a.IsDefault);
    }

    public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.Email)
            ?? throw new AuthenticationException("Invalid authentication.");
    }
}
