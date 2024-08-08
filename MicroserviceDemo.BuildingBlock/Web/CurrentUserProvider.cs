using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MicroserviceDemo.BuildingBlock.Web;
public interface ICurrentUserProvider
{
    string GetUserId();
}

public class CurrentUserProvider(
    IHttpContextAccessor _httpContextAccessor
) : ICurrentUserProvider
{
    public string GetUserId()
    {
        return _httpContextAccessor?.HttpContext?.User
            ?.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}