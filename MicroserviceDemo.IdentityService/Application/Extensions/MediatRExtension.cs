using System.Reflection;
using MicroserviceDemo.BuildingBlock.Caching;

namespace MicroserviceDemo.IdentityService.Application.Extensions;

public static class MediatRExtension
{
    public static IServiceCollection AddCustomMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(CachingRequestBehavior<,>));
            }
        );

        return services;
    }
}