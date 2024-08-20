using MicroserviceDemo.BuildingBlock.Caching;
using Newtonsoft.Json;

namespace MicroserviceDemo.ServiceA.Infrastructure.Service;

public class LoadBalancingService
(
    ICachingHandlerService _cachingHandler,
    IHttpClientFactory _httpClientFactory
) : ILoadBalancingService
{
    public async Task<T> GetData<T>(string path, string serviceKey)
    {
        var host = await _cachingHandler.SortedSetGetLowestScoreAsync<string>(serviceKey);
        var requestUri = $"{host}/{path}";
        await _cachingHandler.SortedSetIncrementAsync<double>(serviceKey, host);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

        var httpClient = _httpClientFactory.CreateClient(typeof(IServiceBClientAPI).ToString());
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var resp = await httpClient.SendAsync(httpRequestMessage);
        await _cachingHandler.SortedSetDecrementAsync<double>(serviceKey, host);
        resp.EnsureSuccessStatusCode();

        var result = await resp.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(result);
    }
    
    public async Task<T?> GetGrpcData<T>(string serviceKey, string host, Func<Task<T>> getGrpcData)
    {
        await _cachingHandler.SortedSetIncrementAsync<double>(serviceKey, host);
        var resp = await getGrpcData();
        await _cachingHandler.SortedSetDecrementAsync<double>(serviceKey, host);

        return resp;
    }

    public async Task<string> GetBestHost(string serviceKey)
    {
        return await _cachingHandler.SortedSetGetLowestScoreAsync<string>(serviceKey);
    }
}