namespace MicroserviceDemo.BuildingBlock.Consul
{
    public class ConsulOptions
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Uri DiscoveryAddress { get; set; }

        public string HealthCheckEndPoint { get; set; }
    }
}
