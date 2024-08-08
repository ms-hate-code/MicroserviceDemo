using MicroserviceDemo.BuildingBlock.RestEase;

namespace MicroserviceDemo.ServiceA.Infrastructure.Service;

public interface ILoadBalancingService
{
    Task<T> GetData<T>(string path, string serviceKey);
    Task<string> GetBestHost(string serviceKey);
    Task<T?> GetGrpcData<T>(string serviceKey, string host, Func<Task<T>> getGrpcData);
}