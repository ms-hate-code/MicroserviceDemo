namespace MicroserviceDemo.LoadBalancer.Application.Models.CustomProxyConfig;

public class UpdateClusterDestinationProxyConfigDto
{
    public string ClusterId { get; set; }
    public string DestinationHost { get; set; }
}