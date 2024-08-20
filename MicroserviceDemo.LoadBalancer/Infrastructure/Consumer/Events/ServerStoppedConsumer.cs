using MassTransit;
using MicroserviceDemo.BuildingBlock.Contracts.Yarp;
using MicroserviceDemo.LoadBalancer.Application.Models.CustomProxyConfig;
using MicroserviceDemo.LoadBalancer.Application.Services.CustomProxyConfig;

namespace MicroserviceDemo.LoadBalancer.Infrastructure.Consumer.Events;

public class ServerStoppedConsumer
(
    ICustomProxyConfigService _customProxyConfig
) : IConsumer<IYarpServerStopped>
{
    public async Task Consume(ConsumeContext<IYarpServerStopped> context)
    {
        var message = context.Message;
        await _customProxyConfig.RemoveHostConfig(new UpdateClusterDestinationProxyConfigDto()
        {
            ClusterId = message.ClusterId,
            DestinationHost = message.Host
        });
    }
}