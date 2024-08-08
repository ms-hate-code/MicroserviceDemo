using IdentityServer4.Contrib.RedisStore;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Services;
using MicroserviceDemo.BuildingBlock.EFCore;
using MicroserviceDemo.BuildingBlock.Web;
using MicroserviceDemo.IdentityService.Application.Constants;
using MicroserviceDemo.IdentityService.Application.Options;
using MicroserviceDemo.IdentityService.Domain.Entity;
using MicroserviceDemo.IdentityService.Infrastructure.Configurations;
using MicroserviceDemo.IdentityService.Infrastructure.DBContext;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using ClientStore = MicroserviceDemo.IdentityService.Infrastructure.Configurations.ClientStore;
using ResourceStore = MicroserviceDemo.IdentityService.Infrastructure.Configurations.ResourceStore;

namespace MicroserviceDemo.IdentityService.Application.Extensions;

public static class IdentityServerExtension
    {
        public static WebApplicationBuilder AddCustomIdentityServer(this WebApplicationBuilder builder)
        {
            var authOptions = builder.Configuration.GetOptions<AuthOptions>(nameof(AuthOptions));
            builder.Services.AddValidateOptions<AuthOptions>();

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            builder.Services
                .AddIdentity<AppUser, IdentityRole>(config =>
                {
                    config.Password.RequiredLength = 1;
                    config.Password.RequireDigit = false;
                    config.Password.RequireNonAlphanumeric = false;
                    config.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            var redisOptions = builder.Configuration.GetOptions<BuildingBlock.Caching.RedisOptions>(nameof(RedisOptions));

            var redisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisOptions.Host);

            builder.Services.AddTransient<ClientStore>();
            builder.Services.AddTransient<ResourceStore>();

            builder.Services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.IssuerUri = authOptions.IssuerUri;
                    options.Caching.ClientStoreExpiration = new TimeSpan(0, 0, 30);
                })
                .AddOperationalStore(options =>
                {
                    options.RedisConnectionMultiplexer = redisConnectionMultiplexer;
                    options.Db = redisOptions.DbNumber;
                    options.KeyPrefix = AppConstants.RedisKeyPrefix.OperationalStore;
                })
                .AddRedisCaching(options =>
                {
                    options.Db = redisOptions.DbNumber;
                    options.KeyPrefix = AppConstants.RedisKeyPrefix.ConfigureStore;
                    options.RedisConnectionMultiplexer = redisConnectionMultiplexer;
                })
                .AddClientStoreCache<ClientStore>()
                .AddResourceStoreCache<ResourceStore>()
                .AddAspNetIdentity<AppUser>()
                .AddResourceOwnerValidator<UserValidator>()
                .AddProfileServiceCache<ProfileService>()
                .AddDeveloperSigningCredential();

            builder.Services.AddScoped<IProfileService, ProfileService>();

            return builder;
        }
    }