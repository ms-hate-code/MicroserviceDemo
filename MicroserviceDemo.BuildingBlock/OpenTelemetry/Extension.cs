using MicroserviceDemo.BuildingBlock.Jaeger;
using MicroserviceDemo.BuildingBlock.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace MicroserviceDemo.BuildingBlock.OpenTelemetry
{
    public static class Extension
    {
        public static IServiceCollection AddCustomOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            var jaegerOptions = configuration.GetOptions<JaegerOptions>(nameof(JaegerOptions));

            services.AddOpenTelemetry()
                .ConfigureResource(b =>
                {
                    b.AddService(jaegerOptions.ServiceName);
                })
                .WithTracing(provider => provider
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(jaegerOptions.ServiceName))
                    .AddAspNetCoreInstrumentation(o => {
                        o.RecordException = true;
                        o.EnrichWithException = (activity, exception) =>
                        {
                            activity.SetTag("exceptionType", exception.GetType().ToString());
                            activity.SetTag("stackTrace", exception.StackTrace);
                        };
                    })
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    // .AddGrpcCoreInstrumentation()
                    // .AddGrpcClientInstrumentation()
                    .AddRedisInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddSource(new ActivitySource(jaegerOptions.ServiceName).Name)
                    .AddSource("MassTransit")
                    .AddOtlpExporter()
                )
                .WithMetrics(provider => provider
                    .AddMeter(jaegerOptions.ServiceName)
                    .AddPrometheusExporter()
                    .ConfigureResource(resource =>
                    {
                        resource.AddService(jaegerOptions.ServiceName);
                    }));

            return services;
        }
    }
}
