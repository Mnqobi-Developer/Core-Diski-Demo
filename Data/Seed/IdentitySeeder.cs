using Core_Diski_Demo.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Core_Diski_Demo.Data.Seed;

public class IdentitySeeder(
    RoleManager<IdentityRole> roleManager,
    UserManager<ApplicationUser> userManager,
    IOptions<AdminSeedOptions> adminSeedOptions)
{
    public async Task SeedAsync()
    {
        const string adminRole = "Admin";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        var settings = adminSeedOptions.Value;
        var adminUser = await userManager.FindByEmailAsync(settings.Email);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = settings.Email,
                Email = settings.Email,
                EmailConfirmed = true,
                FirstName = "Store",
                LastName = "Admin"
            };

            var createResult = await userManager.CreateAsync(adminUser, settings.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create admin user: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }
}
