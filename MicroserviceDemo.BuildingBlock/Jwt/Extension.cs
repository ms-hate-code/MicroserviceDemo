using System.Net;
using System.Security.Claims;
using MicroserviceDemo.BuildingBlock.Constants;
using MicroserviceDemo.BuildingBlock.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace MicroserviceDemo.BuildingBlock.Jwt;

public static class Extension
{
    public static IServiceCollection AddJwt(this IServiceCollection services)
        {
            var jwtOptions = services.GetOptions<JwtOptions>(nameof(JwtOptions));

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddCookie(options => options.SlidingExpiration = true)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = jwtOptions.Authority;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.FromSeconds(2)
                    };
                    options.RequireHttpsMetadata = jwtOptions.RequireHttpsMetadata;
                    options.MetadataAddress = jwtOptions.MetadataAddress;
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            context.HandleResponse();
                            var problemDetail = GetProblemDetails(context.HttpContext,
                                StatusCodes.Status401Unauthorized,
                                ReasonPhrases.GetReasonPhrase(StatusCodes.Status401Unauthorized),
                                "You need the access_token to access this resource.");

                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                            var result = JsonConvert.SerializeObject(problemDetail);
                            context.Response.ContentType = "application/json+problem";
                            await context.Response.WriteAsync(result);
                        },
                        OnForbidden = async context =>
                        {
                            var problemDetail = GetProblemDetails(context.HttpContext,
                                StatusCodes.Status403Forbidden,
                                ReasonPhrases.GetReasonPhrase(StatusCodes.Status403Forbidden),
                                "You are not allowed to access this resource.");

                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                            var result = JsonConvert.SerializeObject(problemDetail);
                            context.Response.ContentType = "application/json+problem";
                            await context.Response.WriteAsync(result);
                        }
                    };
                });

            if (!string.IsNullOrEmpty(jwtOptions.Audience))
            {
                services.AddAuthorizationBuilder()
                    .AddPolicy(CustomIdentityConstants.AuthServer.DEMO_APP, policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.RequireClaim("scope", CustomIdentityConstants.AuthServer.DEMO_APP_SCOPE);
                        policy.RequireClaim(ClaimTypes.Role, CustomIdentityConstants.SystemRole.USER);
                        policy.AuthenticationSchemes = [JwtBearerDefaults.AuthenticationScheme];
                    })
                    .AddPolicy(CustomIdentityConstants.AuthServer.DEMO_ADMIN_APP, policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.RequireClaim("scope", CustomIdentityConstants.AuthServer.DEMO_APP_SCOPE);
                        policy.RequireClaim(ClaimTypes.Role, CustomIdentityConstants.SystemRole.ADMIN);
                        policy.AuthenticationSchemes = [JwtBearerDefaults.AuthenticationScheme];
                    })
                    .AddPolicy(CustomIdentityConstants.AuthServer.DEMO_BOTH_APP, policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.RequireClaim("scope", CustomIdentityConstants.AuthServer.DEMO_APP_SCOPE);
                        policy.RequireClaim(ClaimTypes.Role, CustomIdentityConstants.SystemRole.USER, CustomIdentityConstants.SystemRole.ADMIN);
                        policy.AuthenticationSchemes = [JwtBearerDefaults.AuthenticationScheme];
                    });
            }

            return services;
        }

        private static Microsoft.AspNetCore.Mvc.ProblemDetails GetProblemDetails(HttpContext context, int statusCode, string title, string detail)
        {
            return new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Status = statusCode,
                Detail = detail,
                Title = title,
                Extensions =
                {
                    ["TraceId"] = context.TraceIdentifier
                },
                Instance = context.Request.Path,
                Type = $"https://httpstatuses.com/{statusCode}"
            };
        }
}