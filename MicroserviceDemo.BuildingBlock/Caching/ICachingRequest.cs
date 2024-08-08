namespace MicroserviceDemo.BuildingBlock.Caching;

public interface ICachingRequest
{
    public string CacheKey { get; set; }
    public string HashField { get; set; }
    public TimeSpan ExpiredTime { get; set; }
}