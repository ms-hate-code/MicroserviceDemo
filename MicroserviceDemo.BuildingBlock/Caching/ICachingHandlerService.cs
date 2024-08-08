namespace MicroserviceDemo.BuildingBlock.Caching;

public interface ICachingHandlerService
{
    Task<T> HashGetAsync<T>(string cacheKey, string hashField);
    Task HashSetAsync(string cacheKey, string hashField, object value, TimeSpan? expiration = null);
    Task<bool> SortedSetAddAsync(string key, object value, double score);
    Task<bool> SortedSetRemoveAsync<T>(string key, T value);
    Task<double?> SortedSetScoreAsync<T>(string key, T value);
    Task<T> SortedSetGetLowestScoreAsync<T>(string key);
    Task<double> SortedSetIncrementAsync<T>(string key, object member, long value = 1);
    Task<double> SortedSetDecrementAsync<T>(string key, object member, long value = 1);
}