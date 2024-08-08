using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using MicroserviceDemo.IdentityService.Domain.Entity;
using Microsoft.AspNetCore.Identity;

namespace MicroserviceDemo.IdentityService.Infrastructure.Configurations;

public class ProfileService(
    UserManager<AppUser> _userManager,
    RoleManager<IdentityRole> _roleManager,
    IUserClaimsPrincipalFactory<AppUser> _claimsFactory
) : IProfileService
{

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var sub = context.Subject.GetSubjectId();
        var user = await _userManager.FindByIdAsync(sub) ?? throw new KeyNotFoundException("User not found");
        var principal = await _claimsFactory.CreateAsync(user);

        var claims = principal.Claims.ToList();

        claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();

        claims.Add(new Claim(JwtClaimTypes.Subject, sub));
        claims.Add(new Claim(JwtClaimTypes.Email, user?.Email ?? string.Empty));
        claims.Add(new Claim(JwtClaimTypes.Name, user?.UserName ?? string.Empty));
        claims.Add(new Claim(JwtClaimTypes.ClientId, context.Client?.ClientId ?? string.Empty));

        if (_userManager.SupportsUserRole)
        {
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var roleName in roles)
                {
                    claims.Add(new Claim(JwtClaimTypes.Role, roleName));
                    if (!_roleManager.SupportsRoleClaims) continue;
                    
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        claims.AddRange(await _roleManager.GetClaimsAsync(role));
                    }
                }
            }
        }

        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var sub = context.Subject.GetSubjectId();
        var user = await _userManager.FindByIdAsync(sub);
        context.IsActive = user != null;
    }
}