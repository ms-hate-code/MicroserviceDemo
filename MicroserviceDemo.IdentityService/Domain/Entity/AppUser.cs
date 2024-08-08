using Microsoft.AspNetCore.Identity;

namespace MicroserviceDemo.IdentityService.Domain.Entity;

public class AppUser: IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}