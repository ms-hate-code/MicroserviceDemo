using MassTransit;
using MicroserviceDemo.BuildingBlock.Contracts;
using MicroserviceDemo.BuildingBlock.Contracts.Yarp;
using MicroserviceDemo.LoadBalancer.Application.Models.CustomProxyConfig;
using MicroserviceDemo.LoadBalancer.Application.Services.CustomProxyConfig;

namespace MicroserviceDemo.LoadBalancer.Infrastructure.Consumer.Events;

public class ServerHostedConsumer
(
    ICustomProxyConfigService _customProxyConfig
) : IConsumer<IYarpServerHosted>
{
    public async Task Consume(ConsumeContext<IYarpServerHosted> context)
    {
        var message = context.Message;
        await _customProxyConfig.UpdateHostConfig(new UpdateClusterDestinationProxyConfigDto()
        {
            ClusterId = message.ClusterId,
            DestinationHost = message.Host
        });
    }
}