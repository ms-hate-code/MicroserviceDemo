using Grpc.Core;
using TestBProtoService.Generated;

namespace MicroserviceDemo.ServiceB.Application.Services.gRPC;
public class TestBService : TestBProtoService.Generated.TestBProtoService.TestBProtoServiceBase
{
    public override async Task<GetTestBResponse> GetTestB(GetTestBRequest request, ServerCallContext context)
    {
        var data = $"{context.Host} => gRPC Response from Service B";
        await Task.Delay(3000);
        return new GetTestBResponse
        {
            Message = data
        };
    }
}