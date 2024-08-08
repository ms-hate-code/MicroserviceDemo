using System.Security.Authentication;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using MicroserviceDemo.IdentityService.Domain.Entity;
using Microsoft.AspNetCore.Identity;

namespace MicroserviceDemo.IdentityService.Infrastructure.Configurations;

public class UserValidator(
    UserManager<AppUser> _userManager
) : IResourceOwnerPasswordValidator
{
    public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(context.UserName)
                       ?? throw new InvalidCredentialException("Email is incorrect");

            context.Result = new GrantValidationResult(
                subject: user.Id,
                authenticationMethod: GrantType.ResourceOwnerPassword
            );
        }
        catch (Exception)
        {
            context.Result = new GrantValidationResult(TokenRequestErrors.UnauthorizedClient, "Invalid Credentials");
            throw;
        }
    }
}