using MicroserviceDemo.BuildingBlock.Resiliency;
using MicroserviceDemo.BuildingBlock.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using RestEase;

namespace MicroserviceDemo.BuildingBlock.RestEase
{
    public static class Extension
    {
        public static IServiceCollection RegisterServiceForwarder<T>(this IServiceCollection services, string serviceName)
            where T : class
        {
            var clientName = typeof(T).ToString();
            var options = ConfigureOptions(services);

            ConfigureDefaultClient(services, clientName, serviceName, options);

            ConfigureForwarder<T>(services, clientName);

            return services;
        }

        private static RestEaseOptions ConfigureOptions(IServiceCollection services)
        {
            IConfiguration configuration;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                configuration = serviceProvider.GetService<IConfiguration>();
            }

            services.AddValidateOptions<RestEaseOptions>();

            return configuration.GetOptions<RestEaseOptions>(nameof(RestEaseOptions));
        }

        private static void ConfigureDefaultClient(IServiceCollection services, string clientName, string serviceName,
                       RestEaseOptions options)
        {

            services.AddHttpClient(clientName, c =>
                {
                    var service = (options.Services?.FirstOrDefault(x => x.Name.Equals(serviceName,
                                      StringComparison.InvariantCultureIgnoreCase)))
                                  ?? throw new KeyNotFoundException(
                                      $"RestEase service: '{serviceName}' was not found.");

                    c.BaseAddress = new UriBuilder
                    {
                        Scheme = service.Scheme,
                        Host = service.Host,
                        Port = service.Port
                    }.Uri;
                })
                .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                })
                .AddPolicyHandler(CustomPollyPolicy.GetRetryPolicy())
                .AddPolicyHandler(CustomPollyPolicy.GetCircuitBreakerPolicy())
                .AddPolicyHandler(CustomPollyPolicy.GetRateLimitingPolicy());
        }

        private static void ConfigureForwarder<T>(IServiceCollection services, string clientName) where T : class
        {
            services.AddTransient<T>(c =>
                new RestClient(
                    c.GetService<IHttpClientFactory>()
                     .CreateClient(clientName))
                {
                    RequestQueryParamSerializer = new QueryParamSerializer()
                }.For<T>());
        }
    }
}
