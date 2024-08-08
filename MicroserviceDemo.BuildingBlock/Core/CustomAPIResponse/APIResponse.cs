namespace MicroserviceDemo.BuildingBlock.Core.CustomAPIResponse;

public record APIResponse<T>(int StatusCode, T Data) where T : class
{
}