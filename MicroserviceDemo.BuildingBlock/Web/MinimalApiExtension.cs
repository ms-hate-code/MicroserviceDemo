using System.Reflection;
using MicroserviceDemo.BuildingBlock.Utils;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace MicroserviceDemo.BuildingBlock.Web;

public static class MinimalApiExtension
{
    public static IServiceCollection AddMinimalEndpoints
    (
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped,
        params Assembly[] assemblies
    )
    {
        var scanAssemblies = assemblies.Length != 0
            ? assemblies
            : TypeProvider.GetReferencedAssemblies(Assembly.GetEntryAssembly())
                .Concat(TypeProvider.GetApplicationPartAssemblies(Assembly.GetCallingAssembly()))
                .Distinct()
                .ToArray();

        services.Scan(scan =>
            scan.FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo<IMinimalEndpoint>())
                .UsingRegistrationStrategy(RegistrationStrategy.Append)
                .As<IMinimalEndpoint>()
                .WithLifetime(lifetime)
        );

        return services;
    }


    public static IEndpointRouteBuilder MapMinimalEndpoints
    (
        this IEndpointRouteBuilder builder
    )
    {
        var scope = builder.ServiceProvider.CreateScope();
        var endpoints = scope.ServiceProvider.GetServices<IMinimalEndpoint>();

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return builder;
    }
}