using MassTransit;
using MicroserviceDemo.BuildingBlock.Contracts;

namespace MicroserviceDemo.ServiceA.Infrastructure.Consumer.Events
{
    public class MessageResponsedConsumer : IConsumer<IMessageResponsed>
    {
        public async Task Consume(ConsumeContext<IMessageResponsed> context)
        {
            var message = context.Message.Message;
            Console.WriteLine(message);
        }
    }
}
