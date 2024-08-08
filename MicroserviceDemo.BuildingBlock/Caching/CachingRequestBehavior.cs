using MediatR;

namespace MicroserviceDemo.BuildingBlock.Caching;

public class CachingRequestBehavior<TRequest, TResponse>
(
    ICachingHandlerService _cachingHandler
) : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICachingRequest cachingRequest)
        {
            return await next();
        }
        
        var cacheKey = cachingRequest.CacheKey;
        var hashField = cachingRequest.HashField;
            
        var cachedResponse = await _cachingHandler.HashGetAsync<TResponse>(cacheKey, hashField);
        if (cachedResponse != null)
        {
            return cachedResponse;
        }
        
        var response = await next();
        await _cachingHandler.HashSetAsync(cacheKey, hashField, response);
        
        return response;
    }
}