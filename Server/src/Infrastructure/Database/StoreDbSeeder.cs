using Domain.Products;
using Infrastructure.Authorization;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Authorization;

namespace Infrastructure.Database;

public static class StoreDbSeeder
{
    public static async Task SeedAsync(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, ApplicationDbContext context)
    {
        await SeedRolesAndAdminAsync(userManager, roleManager);
        await SeedPermissionsAsync(context);
        await SeedRolePermissionsAsync(roleManager, context);
        await SeedGendersAsync(context);
    }

    private static async Task SeedRolesAndAdminAsync(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        // 1. Ensure roles exist
        string[] roles = { "Admin", "Manager", "Customer" };

        foreach (string roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new AppRole { Name = roleName, NormalizedName = roleName.ToUpperInvariant() });
            }
        }

        // 2. Ensure admin user exists
        const string adminEmail = "pabbasrikar@gmail.com";
        const string adminUserName = "srikarpabba";
#pragma warning disable S2068 // Credentials should not be hard-coded
        const string adminPassword = "Pa$$w0rd123!";
#pragma warning restore S2068 // Credentials should not be hard-coded

        AppUser? admin = await userManager.FindByNameAsync(adminUserName);
        if (admin is null)
        {
            admin = new AppUser
            {
                UserName = adminUserName,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "Srikar",
                LastName = "Pabba",
                PhoneNumber = "8608708021"
            };

            IdentityResult result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
            else
            {
                throw new InvalidOperationException(
                    string.Join(Environment.NewLine,
                        result.Errors.Select(e => e.Description)));
            }
        }
    }

    private static async Task SeedPermissionsAsync(ApplicationDbContext context)
    {
        foreach ((string name, string description) in PermissionDefinitions.All)
        {
            bool exists = await context.Permissions
                .AnyAsync(x => x.Name == name);

            if (!exists)
            {
                context.Permissions.Add(new Permission
                {
                    Name = name,
                    Description = description
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedRolePermissionsAsync(RoleManager<AppRole> roleManager, ApplicationDbContext context)
    {
        AppRole? adminRole = await roleManager.FindByNameAsync(Roles.Admin);

        if (adminRole is null)
        {
            return;
        }

        List<Guid> existingPermissionIds = await context.RolePermissions
            .Where(x => x.RoleId == adminRole.Id)
            .Select(x => x.PermissionId)
            .ToListAsync();

        List<Permission> permissions = await context.Permissions.ToListAsync();

        IEnumerable<RolePermission> rolePermissions = permissions
            .Where(x => !existingPermissionIds.Contains(x.Id))
            .Select(x => new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = x.Id
            });

        context.RolePermissions.AddRange(rolePermissions);

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Genders are structural reference data, not user-managed content:
    /// storefront and category queries match them by name ("Male",
    /// "Female", "Unisex"), and there is no Gender CRUD. Everything else
    /// (colors, sizes, brands, categories, ...) is created via the admin UI.
    /// </summary>
    private static async Task SeedGendersAsync(ApplicationDbContext context)
    {
        if (await context.Genders.AnyAsync())
        {
            return;
        }

        context.Genders.AddRange(
            new Gender { Name = "Male" },
            new Gender { Name = "Female" },
            new Gender { Name = "Unisex" });

        await context.SaveChangesAsync();
    }
}
