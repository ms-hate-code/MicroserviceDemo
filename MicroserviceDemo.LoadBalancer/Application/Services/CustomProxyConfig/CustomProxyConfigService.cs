using System.ComponentModel.DataAnnotations;
using MicroserviceDemo.BuildingBlock.Web;
using MicroserviceDemo.LoadBalancer.Application.Models;
using MicroserviceDemo.LoadBalancer.Application.Models.CustomProxyConfig;
using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace MicroserviceDemo.LoadBalancer.Application.Services.CustomProxyConfig;

public class CustomProxyConfigService(IProxyConfigProvider provider) : ICustomProxyConfigService
{
    public async Task UpdateHostConfig(UpdateClusterDestinationProxyConfigDto request)
    {
        if (provider
                .GetConfig() is not CustomConfig config)
        {
            return;
        }
        var clusterConfig = config.Clusters.FirstOrDefault(x => x.ClusterId == request.ClusterId)
            ?? throw new ValidationException();

        if (clusterConfig.Destinations != null)
        {
            var isDestinationExisted =
                clusterConfig.Destinations.Any(x => x.Value.Address == request.DestinationHost);
            if (isDestinationExisted)
            {
                return;
            }
        }
        var newClusterConfig = new ClusterConfig()
        {
            ClusterId = clusterConfig.ClusterId,
            LoadBalancingPolicy = clusterConfig.LoadBalancingPolicy,
            Destinations = new Dictionary<string, DestinationConfig>(clusterConfig.Destinations ?? new Dictionary<string, DestinationConfig>())
            {
                { 
                    $"{request.ClusterId}/{Guid.NewGuid():N}", new DestinationConfig
                    {
                        Address = request.DestinationHost,
                        Health = request.DestinationHost + "/api/healths/status"
                    } 
                }
            },
            HealthCheck = clusterConfig.HealthCheck,
            HttpClient = clusterConfig.HttpClient,
            HttpRequest = clusterConfig.HttpRequest,
            SessionAffinity = clusterConfig.SessionAffinity,
            Metadata = clusterConfig.Metadata
        };

        var newClusters = config.Clusters.ToList();
        newClusters.RemoveAll(x => x.ClusterId == request.ClusterId);
        
        newClusters.Add(newClusterConfig);
        var newConfig = new CustomConfig(config.Routes, newClusters);
        
        await UpdateInternal(newConfig);
    }

    public async Task RemoveHostConfig(UpdateClusterDestinationProxyConfigDto request)
    {
        if (provider
                .GetConfig() is not CustomConfig config)
        {
            return;
        }
        var clusterConfig = config.Clusters.FirstOrDefault(x => x.ClusterId == request.ClusterId)
                            ?? throw new ValidationException();
        
        if (clusterConfig.Destinations != null)
        {
            var destinations = (config.Clusters
                    .Select(x => x.Destinations)
                    .FirstOrDefault() ?? throw new ValidationException())
                .ToDictionary(x => x.Key, v => v.Value);
            
            var keyNeedRemove = destinations
                .Where(x => x.Value.Address == request.DestinationHost)
                .Select(x => x.Key)
                .FirstOrDefault() ?? throw new ValidationException();
            
            destinations.Remove(keyNeedRemove);
            
            var newClusterConfig = new ClusterConfig()
            {
                ClusterId = clusterConfig.ClusterId,
                LoadBalancingPolicy = clusterConfig.LoadBalancingPolicy,
                Destinations = new Dictionary<string, DestinationConfig>(destinations),
                HealthCheck = clusterConfig.HealthCheck,
                HttpClient = clusterConfig.HttpClient,
                HttpRequest = clusterConfig.HttpRequest,
                SessionAffinity = clusterConfig.SessionAffinity,
                Metadata = clusterConfig.Metadata
            };
            
            var newClusters = config.Clusters.ToList();
            newClusters.RemoveAll(x => x.ClusterId == request.ClusterId);
            newClusters.Add(newClusterConfig);
            
            var newConfig = new CustomConfig(config.Routes, newClusters);
        
            await UpdateInternal(newConfig);
        }
    }

    private async Task UpdateInternal(CustomConfig newConfig)
    {
        await (provider as CustomProxyConfigProvider)!.UpdateConfig(newConfig);
    }
}

public class CustomProxyConfigProvider: IProxyConfigProvider
{
    private volatile CustomConfig _config;
    public CustomProxyConfigProvider(IConfiguration configuration)
    {
        var reverseProxyConfig = configuration.GetOptions<ReverseProxyConfig>("ReverseProxy");

        var routeConfigs = reverseProxyConfig.Routes
            .Select(route => route.Value)
            .ToList();

        var clusterConfigs = reverseProxyConfig.Clusters
            .Select(cluster => cluster.Value)
            .ToList();

        _config = new CustomConfig(routeConfigs, clusterConfigs);
    }

    public async Task UpdateConfig(CustomConfig newConfig)
    {
        var oldConfig = Interlocked.Exchange(ref _config, newConfig);
        await oldConfig.SignalChange();
    }
    public IProxyConfig GetConfig()
    {
        return _config;
    }
}

public class CustomConfig : IProxyConfig
{
    private readonly CancellationTokenSource _cts = new();
    public CustomConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
    {
        Routes = routes;
        Clusters = clusters;
        ChangeToken = new CancellationChangeToken(_cts.Token);
    }
    public IReadOnlyList<RouteConfig> Routes { get; }
    public IReadOnlyList<ClusterConfig> Clusters { get; }
    public IChangeToken ChangeToken { get; }
    
    public async Task SignalChange()
    {
        await _cts.CancelAsync();
    }
}