using MassTransit;

namespace MicroserviceDemo.BuildingBlock.Contracts.Yarp;

[EntityName("yarp-server-hosted")]
public interface IYarpServerHosted: CorrelatedBy<Guid>
{
    public string ClusterId { get; set; }
    public string Host { get; set; }
}