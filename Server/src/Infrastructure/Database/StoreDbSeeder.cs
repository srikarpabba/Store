using Domain.Products;
using Infrastructure.Authorization;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Authorization;
using Color = Domain.Products.Color;
using Size = Domain.Products.Size;

namespace Infrastructure.Database;

public static class StoreDbSeeder
{
    public static async Task SeedAsync(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, ApplicationDbContext context, IDateTimeProvider dateTimeProvider)
    {
        await SeedRolesAndAdminAsync(userManager, roleManager);
        await SeedPermissionsAsync(context);
        await SeedRolePermissionsAsync(roleManager, context);
        await SeedLookupDataAsync(userManager, context, dateTimeProvider);
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

    private static async Task SeedLookupDataAsync(UserManager<AppUser> userManager, ApplicationDbContext context, IDateTimeProvider dateTimeProvider)
    {
        AppUser? admin = await userManager.FindByEmailAsync("pabbasrikar@gmail.com");
        Guid? adminId = admin?.Id;

        DateTime now = dateTimeProvider.UtcNow;

        if (!await context.Colors.AnyAsync())
        {
            var colors = new List<Color>
            {
                new Color { Name = "Red", HexCode = "#FF0000" },
                new Color { Name = "Blue", HexCode = "#0000FF" },
                new Color { Name = "Green", HexCode = "#00FF00" },
                new Color { Name = "Black", HexCode = "#000000" },
                new Color { Name = "White", HexCode = "#FFFFFF" },
                new Color { Name = "Purple", HexCode = "#800080" },
                new Color { Name = "Pink", HexCode = "#FFC0CB" },
                new Color { Name = "Brown", HexCode = "#A52A2A" },
                new Color { Name = "Gray", HexCode = "#808080" },
                new Color { Name = "Navy Blue", HexCode = "#000080" }
            };
            context.Colors.AddRange(colors);
        }

        if (!await context.Sizes.AnyAsync())
        {
            var sizes = new List<Size>
            {
                new Size { Name = "S" },
                new Size { Name = "M" },
                new Size { Name = "L" },
                new Size { Name = "XL" },
                new Size { Name = "XXL" },
                new Size { Name = "OneSize" },
                new Size { Name = "6" },
                new Size { Name = "7" },
                new Size { Name = "8" },
                new Size { Name = "9" },
                new Size { Name = "10" },
                new Size { Name = "30" },
                new Size { Name = "32" },
                new Size { Name = "34" },
                new Size { Name = "36" },
            };
            context.Sizes.AddRange(sizes);
        }

        if (!await context.Genders.AnyAsync())
        {
            var genders = new List<Gender>
            {
                new Gender { Name = "Male" },
                new Gender { Name = "Female" },
                new Gender { Name = "Unisex" }
            };
            context.Genders.AddRange(genders);
        }

        if (!await context.Brands.AnyAsync())
        {
            var brands = new List<Brand>
            {
                new Brand { Name = "Levi's" , CreatedOnUtc = now, CreatedBy = adminId },
                new Brand { Name = "Tommy Hilfiger", CreatedOnUtc = now, CreatedBy = adminId  },
                new Brand {Name = "Wrangler", CreatedOnUtc = now, CreatedBy = adminId},
                new Brand {Name = "Nike", CreatedOnUtc = now, CreatedBy = adminId},
                new Brand {Name = "Zara", CreatedOnUtc = now, CreatedBy = adminId},
                new Brand {Name = "FabIndia", CreatedOnUtc = now, CreatedBy = adminId},
                new Brand {Name = "Adidas", CreatedOnUtc = now, CreatedBy = adminId},
                new Brand {Name = "Puma", CreatedOnUtc = now, CreatedBy = adminId},
                new Brand {Name = "RedTape", CreatedOnUtc = now, CreatedBy = adminId},
                new Brand {Name = "Clarks", CreatedOnUtc = now, CreatedBy = adminId}
            };
            context.Brands.AddRange(brands);
        }

        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Boots" , CreatedOnUtc = now, CreatedBy = adminId },
                new Category { Name = "Hat" , CreatedOnUtc = now, CreatedBy = adminId },
                new Category { Name = "Kurta" , CreatedOnUtc = now, CreatedBy = adminId },
                new Category { Name = "Dress" , CreatedOnUtc = now, CreatedBy = adminId },
                new Category { Name = "Shoes" , CreatedOnUtc = now, CreatedBy = adminId },
                new Category { Name = "Jeans" , CreatedOnUtc = now, CreatedBy = adminId },
                new Category { Name = "Shirt" , CreatedOnUtc = now, CreatedBy = adminId },
                new Category { Name = "T-Shirt" , CreatedOnUtc = now, CreatedBy = adminId }
            };
            context.Categories.AddRange(categories);
        }

        await context.SaveChangesAsync();
    }
}
