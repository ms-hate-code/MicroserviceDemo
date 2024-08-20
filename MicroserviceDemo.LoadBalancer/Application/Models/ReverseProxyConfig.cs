using Yarp.ReverseProxy.Configuration;

namespace MicroserviceDemo.LoadBalancer.Application.Models;

public class ReverseProxyConfig
{
    public Dictionary<string, RouteConfig> Routes { get; set; }
    public Dictionary<string, ClusterConfig> Clusters { get; set; }
}
