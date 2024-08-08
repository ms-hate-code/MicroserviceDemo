using Grpc.Net.Client;
using MassTransit;
using MicroserviceDemo.BuildingBlock.Caching;
using MicroserviceDemo.BuildingBlock.Constants;
using MicroserviceDemo.BuildingBlock.Contracts;
using MicroserviceDemo.BuildingBlock.Core.CustomAPIResponse;
using MicroserviceDemo.ServiceA.Infrastructure.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestBProtoService.Generated;

namespace MicroserviceDemo.ServiceA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestAController
    (
        ILoadBalancingService _loadBalancingService,
        IPublishEndpoint _publishEndpoint,
        TestBProtoService.Generated.TestBProtoService.TestBProtoServiceClient _protoServiceClient
    ) : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult GetTestData()
        {
            var request = HttpContext.Request;
            var data = $"{request.Host} => Response from Service A";

            return Ok(new APIResponse<string>(StatusCodes.Status200OK, data));
        }

        [HttpGet("forward/load-balancer")]
        public async Task<IActionResult> GetForwardLoadBalancerData()
        {
            var data = await _loadBalancingService.GetData<APIResponse<string>>("api/testB/forward/load-balancer", DistributedCacheKeyConst.ServiceBAddressCacheKey);

            return Ok(data);
        }

        [HttpGet("forward/exception")]
        public async Task<IActionResult> GetForwardExceptionData()
        {
            var data = await _loadBalancingService.GetData<APIResponse<string>>("api/testB/exception", DistributedCacheKeyConst.ServiceBAddressCacheKey);

            return Ok(data);
        }

        [HttpGet("forward/rate-limit")]
        public async Task<IActionResult> GetForwardRateLimitData()
        {
            var data = await _loadBalancingService.GetData<APIResponse<string>>("api/testB/forward/rate-limit", DistributedCacheKeyConst.ServiceBAddressCacheKey);

            return Ok(data);
        }

        [HttpGet("send-message")]
        public async Task<IActionResult> SendMessage()
        {
            await _publishEndpoint.Publish<IMessageCreated>(new {
                Message = "Message created"
            });

            return Ok();
        }

        [HttpGet("identity")]
        [Authorize(Policy = $"{CustomIdentityConstants.AuthServer.DEMO_APP}")]
        public async Task<IActionResult> GetGrpcData()
        {
            var data = "Authorized endpoint";

            return Ok(new APIResponse<string>(StatusCodes.Status200OK, data));
        }

        [HttpGet("grpc")]
        public async Task<IActionResult> GetIdentityData()
        {
            var host = await _loadBalancingService.GetBestHost(DistributedCacheKeyConst.ServiceBAddressCacheKey);
            var data = await _loadBalancingService.GetGrpcData(DistributedCacheKeyConst.ServiceBAddressCacheKey, host,
                async () =>
                {
                    var httpHandler = new HttpClientHandler();  
                    httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; 
                    using var channel = GrpcChannel.ForAddress(host, new GrpcChannelOptions { HttpHandler = httpHandler });
                    _protoServiceClient = new TestBProtoService.Generated.TestBProtoService.TestBProtoServiceClient(channel);
                    return await _protoServiceClient.GetTestBAsync(new GetTestBRequest());
                });

            return Ok(new APIResponse<string>(StatusCodes.Status200OK, data?.Message));
        }
    }
}
