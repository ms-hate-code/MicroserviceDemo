using MassTransit;

namespace MicroserviceDemo.BuildingBlock.Contracts.Yarp;

[EntityName("yarp-server-stopped")]
public interface IYarpServerStopped: CorrelatedBy<Guid>
{
    public string ClusterId { get; set; }
    public string Host { get; set; }
}