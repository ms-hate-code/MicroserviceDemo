using System.Net;
using MicroserviceDemo.BuildingBlock.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Hosting;

namespace MicroserviceDemo.BuildingBlock.gRPC;

public static class Extension
{
    public static WebApplicationBuilder UseCustomKestrelGrpc(this WebApplicationBuilder builder)
    {
        var host = GetCurrentHost();
        
        builder.WebHost.UseKestrel(options =>
        {
            var isDevEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            if (isDevEnv)
            {
                options.ListenLocalhost(host.Port, listenOptions =>
                {
                    listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
                    listenOptions.UseHttps();
                });
            }
            else
            {
                options.ListenAnyIP(host.Port, listenOptions =>
                {
                    listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
                    listenOptions.UseHttps();
                });
            }
        });
        return builder;
    }
    
    private static Uri GetCurrentHost()
    {
        var iPAddress = GlobalExtension.GetCurrentHost();
        
        return new Uri(iPAddress);
    }
}