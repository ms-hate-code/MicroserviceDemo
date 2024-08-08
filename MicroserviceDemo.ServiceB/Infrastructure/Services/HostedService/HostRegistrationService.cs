using MicroserviceDemo.BuildingBlock.Caching;
using MicroserviceDemo.BuildingBlock.Web;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace MicroserviceDemo.ServiceB.Infrastructure.Services.HostedService;

public class HostRegistrationService
(
    IHostApplicationLifetime _lifetime,
    IServer server,
    IServiceScopeFactory _scopeFactory
) : IHostedService
{
    private ICachingHandlerService _cachingHandler;
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        _cachingHandler = scope.ServiceProvider
            .GetRequiredService<ICachingHandlerService>();
        
        _lifetime.ApplicationStarted.Register(OnStarted);
        _lifetime.ApplicationStopping.Register(OnStopping);
        _lifetime.ApplicationStopped.Register(OnStopped);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
    }
        
    private void OnStarted()
    {
        var currentHost = GlobalExtension.GetCurrentHost();
        _cachingHandler.SortedSetAddAsync(DistributedCacheKeyConst.ServiceBAddressCacheKey, currentHost, 0);
    }
        
    private void OnStopping()
    {
        var currentHost = GlobalExtension.GetCurrentHost();
        _cachingHandler.SortedSetRemoveAsync(DistributedCacheKeyConst.ServiceBAddressCacheKey, currentHost);
        Thread.Sleep(5000);
    }

    private void OnStopped()
    {
        var currentHost = GlobalExtension.GetCurrentHost();
        _cachingHandler.SortedSetRemoveAsync(DistributedCacheKeyConst.ServiceBAddressCacheKey, currentHost);
    }
    
    private string GetCurrentHost()
    {
        var features = server.Features;
        var addresses = features.Get<IServerAddressesFeature>();
        var iPAddress = addresses?.Addresses.FirstOrDefault();
        
        return iPAddress;
    }
}