using Microsoft.AspNetCore.Routing;

namespace MicroserviceDemo.BuildingBlock.Web;

public interface IMinimalEndpoint
{
    IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder);
}