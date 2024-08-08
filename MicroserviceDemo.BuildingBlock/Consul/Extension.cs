using Consul;
using MicroserviceDemo.BuildingBlock.Web;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MicroserviceDemo.BuildingBlock.Consul
{
    public static class Extensions
    {
        public static void AddConsul(this IServiceCollection services)
        {
            var consulOptions = services.GetOptions<ConsulOptions>(nameof(ConsulOptions));
            ArgumentNullException.ThrowIfNull(consulOptions);

            Console.WriteLine(consulOptions.DiscoveryAddress);
            var consulClient = new ConsulClient(config =>
            {
                config.Address = consulOptions.DiscoveryAddress;
            });

            services.AddSingleton(consulOptions);
            services.AddSingleton<IConsulClient, ConsulClient>(_ => consulClient);
            services.AddSingleton<IHostedService, ConsulHostedService>();
        }
    }
}
