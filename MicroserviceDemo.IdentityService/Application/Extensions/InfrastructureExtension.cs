using MicroserviceDemo.BuildingBlock.Caching;
using MicroserviceDemo.BuildingBlock.Consul;
using MicroserviceDemo.BuildingBlock.EFCore;
using MicroserviceDemo.BuildingBlock.Jwt;
using MicroserviceDemo.BuildingBlock.OpenTelemetry;
using MicroserviceDemo.BuildingBlock.ProblemDetails;
using MicroserviceDemo.BuildingBlock.Swagger;
using MicroserviceDemo.BuildingBlock.Web;
using MicroserviceDemo.IdentityService.Application.Options;
using MicroserviceDemo.IdentityService.Infrastructure.DBContext;
using MicroserviceDemo.IdentityService.Infrastructure.DBContext.Seed;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;

namespace MicroserviceDemo.IdentityService.Application.Extensions;

public static class InfrastructureExtension
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
        
        builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddConsul();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();
        builder.Services.AddCustomDbContext<IdentityContext>();
        builder.Services.AddScoped<IDataSeeder, IdentityDataSeeder>();
        builder.Services.AddCustomSwagger();
        builder.Services.AddCustomRedisCaching();
        builder.Services.AddCustomOpenTelemetry(configuration);
        // builder.Services.AddCustomAPIVersioning();
        // builder.Services.AddValidatorsFromAssembly(typeof(IdentityRoot).Assembly);
        builder.Services.AddCustomMediatR();
        builder.Services.AddProblemDetails();
        // builder.Services.AddCustomMapster(typeof(IdentityRoot).Assembly);
        // builder.Services.AddCustomHealthCheck();

        builder.AddCustomIdentityServer();
        builder.Services.AddJwt();

        var authOptions = builder.Services.GetOptions<AuthOptions>(nameof(AuthOptions));
        builder.Services.AddHttpClient(authOptions.ClientId, c => { c.BaseAddress = new Uri(authOptions.IssuerUri); }
        );
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        return builder;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseCustomProblemDetails();
        app.UseForwardedHeaders();
        app.UseCustomSwagger();

        app.UseMigration<IdentityContext>();
        app.UseIdentityServer();
        app.MapControllers();

        app.MapGet("/api/healths/status", x => x.Response.WriteAsync("Ok!!!"));

        // app.MapGet("/", x => x.Response.WriteAsync(appOptions.Name));

        return app;
    }
}