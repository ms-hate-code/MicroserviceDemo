using MassTransit;
using MicroserviceDemo.BuildingBlock.Contracts;
using MicroserviceDemo.BuildingBlock.Contracts.Yarp;
using MicroserviceDemo.BuildingBlock.Web;

namespace MicroserviceDemo.ServiceA.Infrastructure.Service;

public class HostRegistrationService
(
    IServiceProvider _serviceProvider,
    IHostApplicationLifetime _applicationLifetime
) : BackgroundService
{
    private IPublishEndpoint _publishEndpoint;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        _publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        
        _applicationLifetime.ApplicationStarted.Register(() => OnStarted(stoppingToken));
        _applicationLifetime.ApplicationStopping.Register(() => OnStopping(stoppingToken));
        _applicationLifetime.ApplicationStopped.Register(() => OnStopped());
    }

    private async Task OnStarted(CancellationToken stoppingToken)
    {
        var currentHost = GlobalExtension.GetCurrentHost();
        await _publishEndpoint.Publish<IYarpServerHosted>(new {
            ClusterId = "serviceACluster",
            Host = currentHost
        }, stoppingToken);
    }
        
    private async Task OnStopping(CancellationToken stoppingToken)
    {
        var currentHost = GlobalExtension.GetCurrentHost();
        await _publishEndpoint.Publish<IYarpServerStopped>(new {
            ClusterId = "serviceACluster",
            Host = currentHost
        }, stoppingToken);
        Thread.Sleep(5000);
    }

    private void OnStopped()
    {
        var currentHost = GlobalExtension.GetCurrentHost();
    }
}