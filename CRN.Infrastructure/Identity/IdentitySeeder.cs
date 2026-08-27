using Microsoft.AspNetCore.Identity;

namespace CRN.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        bool includeTestUser = false)
    {
        const string userRole = "User";
        const string adminRole = "Admin";

        // Ensure roles exist.
        if (!await roleManager.RoleExistsAsync(userRole))
        {
            await roleManager.CreateAsync(
                new IdentityRole(userRole));
        }

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(
                new IdentityRole(adminRole));
        }

        // Find the existing development admin account.
        var adminUser = await userManager.FindByNameAsync("admin");

        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@crn.local",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(
                adminUser,
                "Admin@12345");

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(
                        "; ",
                        result.Errors.Select(e => e.Description)));
            }
        }

        // Make sure the admin account has the Admin role.
        if (!await userManager.IsInRoleAsync(
                adminUser,
                adminRole))
        {
            await userManager.AddToRoleAsync(
                adminUser,
                adminRole);
        }

        if (includeTestUser)
        {
            var testUser = await userManager.FindByNameAsync("testuser");

            if (testUser is null)
            {
                testUser = new ApplicationUser
                {
                    UserName = "testuser",
                    Email = "testuser@crn.local",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(
                    testUser,
                    "User@12345");

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join(
                            "; ",
                            result.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(testUser, userRole))
            {
                await userManager.AddToRoleAsync(
                    testUser,
                    userRole);
            }
        }
    }
}