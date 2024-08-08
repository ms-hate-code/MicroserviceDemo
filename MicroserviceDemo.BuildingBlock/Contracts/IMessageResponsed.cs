using MassTransit;

namespace MicroserviceDemo.BuildingBlock.Contracts
{
    [EntityName("message-responsed")]
    public interface IMessageResponsed : CorrelatedBy<Guid>
    {
        public string Message { get; set; }
    }
}
