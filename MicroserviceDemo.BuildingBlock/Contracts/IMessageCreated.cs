using MassTransit;

namespace MicroserviceDemo.BuildingBlock.Contracts
{
    [EntityName("message-created")]
    public interface IMessageCreated : CorrelatedBy<Guid>
    {
        public string Message { get; set; }
    }
}
