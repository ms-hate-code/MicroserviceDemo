using MassTransit;
using MicroserviceDemo.BuildingBlock.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MicroserviceDemo.BuildingBlock.MassTransit
{
    public static class Extension
    {
        public static IServiceCollection AddCustomMassTransit(this IServiceCollection services, IConfiguration configuration)
        {

            services
                .AddMassTransit(mt =>
                {
                    var assembly = Assembly.GetEntryAssembly();

                    mt.AddConsumers(assembly);
                    mt.AddSagaStateMachines(assembly);
                    mt.AddSagas(assembly);
                    mt.AddActivities(assembly);

                    mt.SetKebabCaseEndpointNameFormatter();

                    mt.UsingRabbitMq((context, cfg) =>
                    {
                        var rabbitMqSettings = configuration.GetOptions<RabbitMQOptions>(nameof(RabbitMQOptions));

                        cfg.Host(rabbitMqSettings.Uri, "/", h =>
                        {
                            h.Username(rabbitMqSettings.UserName);
                            h.Password(rabbitMqSettings.Password);
                        });

                        cfg.ConfigureEndpoints(context);
                    });
                });

            return services;
        }
    }
}
