using MicroserviceDemo.LoadBalancer.Application.Models.CustomProxyConfig;

namespace MicroserviceDemo.LoadBalancer.Application.Services.CustomProxyConfig;

public interface ICustomProxyConfigService
{
    Task UpdateHostConfig(UpdateClusterDestinationProxyConfigDto request);
    Task RemoveHostConfig(UpdateClusterDestinationProxyConfigDto request);
}