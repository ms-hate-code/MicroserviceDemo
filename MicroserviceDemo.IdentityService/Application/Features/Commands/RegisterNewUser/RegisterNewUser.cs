using Mapster;
using MediatR;
using MicroserviceDemo.BuildingBlock.Constants;
using MicroserviceDemo.BuildingBlock.Core.CQRS;
using MicroserviceDemo.BuildingBlock.Core.CustomAPIResponse;
using MicroserviceDemo.BuildingBlock.Web;
using MicroserviceDemo.IdentityService.Domain.Entity;
using Microsoft.AspNetCore.Identity;

namespace MicroserviceDemo.IdentityService.Application.Features.Commands.RegisterNewUser;

public class RegisterNewUser
{
     public record RegisterNewUserCommand(
        string Email,
        string Password,
        string ConfirmPassword,
        string FirstName,
        string LastName
    ) : ICommand<RegisterNewUserResult>;

    public record RegisterNewUserResult(
        string Id,
        string Email,
        string FirstName,
        string LastName
    );

    private record RegisterNewUserRequestDto(
        string Email,
        string Password,
        string ConfirmPassword,
        string FirstName,
        string LastName
    );

    private record RegisterNewUserResponseDto(
        string Id,
        string Email,
        string FirstName,
        string LastName
    );

    public class RegisterNewUserEndpoint : IMinimalEndpoint
    {
        public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPost($"api/identity/auth/register",
                    async (RegisterNewUserRequestDto request, IMediator _mediator) =>
                    {
                        var command = request.Adapt<RegisterNewUserCommand>();

                        var result = await _mediator.Send(command);

                        var response = result.Adapt<RegisterNewUserResponseDto>();

                        return Results.Ok(
                            new APIResponse<RegisterNewUserResponseDto>(StatusCodes.Status200OK, response));
                    }
                )
                .WithName("RegisterUser")
                .Produces<RegisterNewUserResponseDto>()
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Register new User")
                .WithDescription("Register new user")
                .WithOpenApi();

            return builder;
        }
    }

    public class RegisterNewUserCommandHandler(
        UserManager<AppUser> _userManager
    )
    : ICommandHandler<RegisterNewUserCommand, RegisterNewUserResult>
    {
        public async Task<RegisterNewUserResult> Handle(RegisterNewUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var user = command.Adapt<AppUser>();
                user.UserName = command.Email.Replace("@", "");
                var result = await _userManager.CreateAsync(user, command.Password);

                if (result.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, CustomIdentityConstants.SystemRole.USER);

                    if (!roleResult.Succeeded)
                    {
                        throw new Exception(string.Join(',', roleResult.Errors.Select(e => e.Description)));
                    }

                    return user.Adapt<RegisterNewUserResult>();
                }

                throw new BadHttpRequestException(string.Join(',', result.Errors.Select(e => e.Description)));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}