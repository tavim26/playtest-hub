using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PlaytestHub.Modules.Identity.Contracts;
using PlaytestHub.Modules.Identity.Domain;
using PlaytestHub.Shared.Infrastructure.Database;

namespace PlaytestHub.Modules.Identity.Infrastructure.Database;

internal sealed class IdentitySeeder(
    UserManager<User> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IConfiguration configuration,
    ILogger<IdentitySeeder> logger) : IDataSeeder
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (!await roleManager.RoleExistsAsync(Roles.Moderator))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(Roles.Moderator));
        }

        var email = configuration["Seed:AdminEmail"];
        var password = configuration["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        if (await userManager.FindByEmailAsync(email) is not null)
        {
            return;
        }

        var admin = new User
        {
            UserName = email,
            Email = email,
            DisplayName = "Platform Admin",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, password);

        if (!result.Succeeded)
        {
            logger.LogWarning("Admin user could not be seeded: {Errors}",
                string.Join(" ", result.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(admin, Roles.Moderator);
        logger.LogInformation("Seeded admin user {Email}", email);
    }
}