using Mapster;
using MediatR;
using MicroserviceDemo.BuildingBlock.Caching;
using MicroserviceDemo.BuildingBlock.Constants;
using MicroserviceDemo.BuildingBlock.Core.CQRS;
using MicroserviceDemo.BuildingBlock.Core.CustomAPIResponse;
using MicroserviceDemo.BuildingBlock.Web;
using MicroserviceDemo.IdentityService.Domain.Entity;
using Microsoft.AspNetCore.Identity;

namespace MicroserviceDemo.IdentityService.Application.Features.Queries.GetCurrentUser;

public record GetUserProfileQuery(string UserId) : IQuery<GetUserProfileResult>, ICachingRequest
{
    public string CacheKey { get; set; } = "IdentityService_CurrentUser";
    public string HashField { get; set; } = UserId;
    public TimeSpan ExpiredTime { get; set; }
}

public record GetUserProfileResult(
    string Id,
    string Email,
    string PhoneNumber,
    string FirstName,
    string LastName
);

public record GetUserProfileResponseDto(
    string Id,
    string Email,
    string PhoneNumber,
    string FirstName,
    string LastName
);

public class GetUserProfileEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"api/identity/profile", async
                (ICurrentUserProvider _currentUserProvider, IMediator _mediator) =>
            {
                var query = new GetUserProfileQuery(_currentUserProvider.GetUserId());

                var result = await _mediator.Send(query);

                var response = result.Adapt<GetUserProfileResponseDto>();

                return Results.Ok(new APIResponse<GetUserProfileResponseDto>(StatusCodes.Status200OK, response));
            })
            .RequireAuthorization(CustomIdentityConstants.AuthServer.DEMO_APP)
            .WithName("GetUserProfile")
            .Produces<GetUserProfileResponseDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get Current User")
            .WithDescription("Get Current User")
            .WithOpenApi();


        return builder;
    }
}

public class GetCurrenUserQueryHandler(
    UserManager<AppUser> _userManager
)
: IQueryHandler<GetUserProfileQuery, GetUserProfileResult>
{
    public async Task<GetUserProfileResult> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId)
                ?? throw new KeyNotFoundException("Cannot find current user");

            return user.Adapt<GetUserProfileResult>();
        }
        catch (Exception)
        {
            throw;
        }
    }
}