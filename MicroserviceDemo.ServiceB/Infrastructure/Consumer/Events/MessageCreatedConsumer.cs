using MassTransit;
using MicroserviceDemo.BuildingBlock.Contracts;

namespace MicroserviceDemo.ServiceB.Infrastructure.Consumer.Events
{
    public class MessageCreatedConsumer
    (
        IPublishEndpoint _publishEndpoint
    ) : IConsumer<IMessageCreated>
    {
        public async Task Consume(ConsumeContext<IMessageCreated> context)
        {
            var message = context.Message.Message;
            Console.WriteLine(message);

            await _publishEndpoint.Publish<IMessageResponsed>(new
            {
                Message = "Message responsed"
            });
        }
    }
}
