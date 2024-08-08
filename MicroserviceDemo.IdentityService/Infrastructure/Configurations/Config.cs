using IdentityServer4.Models;
using MicroserviceDemo.IdentityService.Application.Options;
using Microsoft.Extensions.Options;

namespace MicroserviceDemo.IdentityService.Infrastructure.Configurations;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResources.Email(),
        new IdentityResources.Phone()
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new ApiScope("microservice_demo_app_api")
    ];

    public static IEnumerable<ApiResource> ApiResources =>
    [
        new ApiResource("identity")
    ];
}