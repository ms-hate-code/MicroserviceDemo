namespace MicroserviceDemo.IdentityService.Application.Constants;

public static class AppConstants
{
    public static class  IdentityServer4
    {
        public const string RequestToken = "connect/token";
    }
    
    public static class StandardScopes
    {
        public const string IdentityApi = "identity-api";
    }

    public static class RedisKeyPrefix
    {
        public const string OperationalStore = "Identity:Operational";
        public const string ConfigureStore = "Identity:ConfigureStore";
    }
}