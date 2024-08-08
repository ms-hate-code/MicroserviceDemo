using IdentityModel.Client;
using IdentityServer4.Models;
using Mapster;
using MassTransit;
using MediatR;
using MicroserviceDemo.BuildingBlock.Core.CQRS;
using MicroserviceDemo.BuildingBlock.Core.CustomAPIResponse;
using MicroserviceDemo.BuildingBlock.Jwt;
using MicroserviceDemo.BuildingBlock.Web;
using MicroserviceDemo.IdentityService.Application.Constants;
using MicroserviceDemo.IdentityService.Application.Options;
using MicroserviceDemo.IdentityService.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace MicroserviceDemo.IdentityService.Application.Features.Commands.LoginUser;

public class LoginUser
    {
        public record LoginUserCommand(
            string Email,
            string Password
        ) : ICommand<LoginUserResult>;

        public record LoginUserResult(
            string AccessToken,
            string RefreshToken,
            int ExpiresIn,
            string Scope
        );

        public record LoginUserRequestDto(
            string Email,
            string Password
        );

        public record LoginUserResponseDto(
            string AccessToken,
            string RefreshToken,
            int ExpiresIn,
            string Scope
        );

        public class LoginUserEndpoint : IMinimalEndpoint
        {
            public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
            {
                builder.MapPost($"api/identity/auth/login",
                        async (LoginUserRequestDto request, IMediator _mediator) =>
                        {
                            var command = request.Adapt<LoginUserCommand>();

                            var result = await _mediator.Send(command);

                            var response = result.Adapt<LoginUserResponseDto>();

                            return Results.Ok(new APIResponse<LoginUserResponseDto>(StatusCodes.Status200OK, response));
                        }
                    )
                    .WithName("LoginUser")
                    .Produces<LoginUserResponseDto>()
                    .ProducesProblem(StatusCodes.Status400BadRequest)
                    .WithSummary("Login User")
                    .WithDescription("Login user")
                    .WithOpenApi();

                return builder;
            }
        }

        public class LoginUserCommandHandler(
            SignInManager<AppUser> _signInManager,
            UserManager<AppUser> _userManager,
            IHttpClientFactory _httpClientFactory,
            IOptions<AuthOptions> _authOptions,
            IOptions<JwtOptions> _jwtOptions
        )
        : ICommandHandler<LoginUserCommand, LoginUserResult>
        {
            public async Task<LoginUserResult> Handle(LoginUserCommand command, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _userManager.FindByEmailAsync(command.Email)
                        ?? throw new BadHttpRequestException("Email is incorrect");

                    var signInResult = await _signInManager.CheckPasswordSignInAsync(user, command.Password, false);

                    if (!signInResult.Succeeded)
                    {
                        throw new BadHttpRequestException("Password is incorrect");
                    }

                    var authValue = _authOptions.Value;

                    var client = _httpClientFactory.CreateClient(authValue.ClientId);

                    var disco = await client.GetDiscoveryDocumentAsync(cancellationToken: cancellationToken, request: new DiscoveryDocumentRequest()
                    {
                        Address = _jwtOptions.Value.MetadataAddress,
                    });
                    
                    var tokenRequest = new PasswordTokenRequest
                    {
                        Address = $"{authValue.IssuerUri}/{AppConstants.IdentityServer4.RequestToken}",
                        GrantType = GrantType.ResourceOwnerPassword,
                        UserName = command.Email,
                        Password = command.Password,
                        ClientId = authValue.ClientId,
                        ClientSecret = authValue.ClientSecret,
                        Scope = $"offline_access {authValue.Scope}",
                        
                    };

                    var response = await client.RequestPasswordTokenAsync(tokenRequest, cancellationToken);

                    if (response.IsError)
                    {
                        throw new Exception(response.Error);
                    }

                    return new LoginUserResult(
                            response.AccessToken,
                            response.RefreshToken,
                            response.ExpiresIn,
                            response.Scope
                        );
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }