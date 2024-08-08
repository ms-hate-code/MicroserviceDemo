using MicroserviceDemo.BuildingBlock.Caching;
using MicroserviceDemo.BuildingBlock.Core.CustomAPIResponse;
using RestEase;

namespace MicroserviceDemo.ServiceA.Infrastructure.Service
{
    public interface IServiceBClientAPI
    {
        [Get("api/testB/forward/test")]
        Task<APIResponse<string>> GetTestData();
    }
}
