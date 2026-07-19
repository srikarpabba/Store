using Infrastructure.Database;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        using ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync();

        UserManager<AppUser> userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        RoleManager<AppRole> roleManager =
            scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

        IHostEnvironment environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

        if (environment.IsDevelopment())
        {
            await StoreDbSeeder.SeedAsync(userManager, roleManager, dbContext);
        }
    }
}
