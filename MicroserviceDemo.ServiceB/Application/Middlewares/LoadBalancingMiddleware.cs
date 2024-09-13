using MicroserviceDemo.BuildingBlock.Caching;

namespace MicroserviceDemo.ServiceB.Application.Middlewares;

public class LoadBalancingMiddleware
{
    private readonly RequestDelegate _next;

    public LoadBalancingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, ICachingHandlerService _cachingHandlerService)
    {
        var currentRequest = httpContext.Request;
        var currentHost = $"{currentRequest.Scheme}://{currentRequest.Host}";
        await _cachingHandlerService.SortedSetIncrementAsync<double>(DistributedCacheKeyConst.ServiceBAddressCacheKey, currentHost);
        await _next(httpContext);
        await _cachingHandlerService.SortedSetDecrementAsync<double>(DistributedCacheKeyConst.ServiceBAddressCacheKey, currentHost);
    }
}