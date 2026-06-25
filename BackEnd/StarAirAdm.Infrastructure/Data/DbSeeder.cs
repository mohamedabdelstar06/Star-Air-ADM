using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StarAirAdm.Domain.Entities;
using StarAirAdm.Domain.Enums;

namespace StarAirAdm.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var config = serviceProvider.GetRequiredService<IConfiguration>();

        string[] roleNames = { "Admin", "Pilot" };

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var adminEmail = config["SeedData:AdminEmail"] ?? "admin@starair.com";
        var adminPassword = config["SeedData:AdminPassword"] ?? "Admin@123!";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var newAdmin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Star Admin",
                Status = UserStatus.Active,
                EmailConfirmed = true
            };

            var createPowerUser = await userManager.CreateAsync(newAdmin, adminPassword);
            if (createPowerUser.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "Admin");
            }
        }
        else 
        {
            // If user already exists, ensure they have the Admin role
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            adminUser.Status = UserStatus.Active;
            await userManager.UpdateAsync(adminUser);
        }

        var pilotEmail = config["SeedData:PilotEmail"] ?? "pilot@starair.com";
        var pilotPassword = config["SeedData:PilotPassword"] ?? "Pilot@123!";
        var pilotUser = await userManager.FindByEmailAsync(pilotEmail);

        if (pilotUser == null)
        {
            var newPilot = new ApplicationUser
            {
                UserName = pilotEmail,
                Email = pilotEmail,
                FullName = "Star Pilot",
                Status = UserStatus.Active,
                EmailConfirmed = true,
                LicenseNumber = "PL-123456",
                Rank = "Captain",
                MedicalClass = "Class 1"
            };

            var createPilot = await userManager.CreateAsync(newPilot, pilotPassword);
            if (createPilot.Succeeded)
            {
                await userManager.AddToRoleAsync(newPilot, "Pilot");
            }
        }
    }
}
