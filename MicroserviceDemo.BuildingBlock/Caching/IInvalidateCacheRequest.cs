namespace MicroserviceDemo.BuildingBlock.Caching;

public interface IInvalidateCacheRequest
{
    public Dictionary<string, string> CacheKeys { get; set; }
    public DateTime ExpiredTime { get; set; }
}