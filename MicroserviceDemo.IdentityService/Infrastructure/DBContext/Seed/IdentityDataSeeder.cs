using MicroserviceDemo.BuildingBlock.Constants;
using MicroserviceDemo.BuildingBlock.EFCore;
using MicroserviceDemo.IdentityService.Domain.Entity;
using Microsoft.AspNetCore.Identity;

namespace MicroserviceDemo.IdentityService.Infrastructure.DBContext.Seed;

public class IdentityDataSeeder(
    UserManager<AppUser> _userManager,
    RoleManager<IdentityRole> _roleManager
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        await SeedRoles();

        await SeedUsers();
    }

    private async Task SeedRoles()
    {
        if (await _roleManager.RoleExistsAsync(CustomIdentityConstants.SystemRole.ADMIN) == false)
        {
            await _roleManager.CreateAsync(new IdentityRole { Name = CustomIdentityConstants.SystemRole.ADMIN });
        }

        if (await _roleManager.RoleExistsAsync(CustomIdentityConstants.SystemRole.USER) == false)
        {
            await _roleManager.CreateAsync(new IdentityRole { Name = CustomIdentityConstants.SystemRole.USER });
        }
    }

    private async Task SeedUsers()
    {
        if (await _userManager.FindByEmailAsync("admin@admin.com") == null)
        {
            var result = await _userManager.CreateAsync(IdentityInitialData.Users.First(), "Admin@123456");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(IdentityInitialData.Users.First(), CustomIdentityConstants.SystemRole.ADMIN);
            }
        }

        if (await _userManager.FindByNameAsync("user@user.com") == null)
        {
            var result = await _userManager.CreateAsync(IdentityInitialData.Users.Last(), "User@123456");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(IdentityInitialData.Users.Last(), CustomIdentityConstants.SystemRole.USER);
            }
        }
    }
}