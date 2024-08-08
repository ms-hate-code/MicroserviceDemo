using MicroserviceDemo.BuildingBlock.Core.CustomAPIResponse;
using Microsoft.AspNetCore.Mvc;

namespace MicroserviceDemo.ServiceB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestBController : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult GetTestData()
        {
            var request = HttpContext.Request;
            var data = $"{request.Host} => Response from Service B";
            
            return Ok(new APIResponse<string>(StatusCodes.Status200OK, data));
        }
        
        [HttpGet("forward/load-balancer")]
        public async Task<IActionResult> GetData()
        {
            var request = HttpContext.Request;
            var data = $"{request.Host} => Response from Service B";
            await Task.Delay(3000);
            return Ok(new APIResponse<string>(StatusCodes.Status200OK, data));
        }
        
        [HttpGet("forward/rate-limit")]
        public async Task<IActionResult> GetRateLimitData()
        {
            var data = $"Response from Service B";
            return Ok(new APIResponse<string>(StatusCodes.Status200OK, data));
        }
    }
}
