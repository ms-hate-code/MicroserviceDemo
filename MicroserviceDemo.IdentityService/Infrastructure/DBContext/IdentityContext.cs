using System.Reflection;
using MicroserviceDemo.IdentityService.Domain.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MicroserviceDemo.IdentityService.Infrastructure.DBContext;

public class IdentityContext
(
    DbContextOptions<IdentityContext> options
) : IdentityDbContext<AppUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        foreach (var type in builder.Model.GetEntityTypes())
        {
            var tableName = type.GetTableName() ?? "";
            if (tableName.StartsWith("AspNet"))
            {
                type.SetTableName(tableName.Substring(6));
            }
        }
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}