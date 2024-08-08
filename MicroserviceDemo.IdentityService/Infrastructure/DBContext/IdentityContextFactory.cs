using MicroserviceDemo.BuildingBlock.EFCore;
using MicroserviceDemo.BuildingBlock.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MicroserviceDemo.IdentityService.Infrastructure.DBContext;

public class IdentityContextFactory : IDesignTimeDbContextFactory<IdentityContext>
{
    public IdentityContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .Build();

        var postgresOptions = configuration.GetOptions<PostgresOptions>(nameof(PostgresOptions));

        var optionBuilder = new DbContextOptionsBuilder<IdentityContext>();
        optionBuilder.UseNpgsql(postgresOptions.ConnectionString)
            .UseSnakeCaseNamingConvention();

        return new IdentityContext(optionBuilder.Options);
    }
}