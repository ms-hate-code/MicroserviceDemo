﻿
namespace MicroserviceDemo.BuildingBlock.MassTransit
{
    internal class RabbitMQOptions
    {
        public string Uri { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
