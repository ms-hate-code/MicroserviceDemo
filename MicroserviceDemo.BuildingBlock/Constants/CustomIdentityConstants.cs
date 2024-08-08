namespace MicroserviceDemo.BuildingBlock.Constants;

public class CustomIdentityConstants
{
    public static class AuthServer
    {
        public const string DEMO_APP = "DEMO_APP";
        public const string DEMO_ADMIN_APP = "DEMO_ADMIN_APP";
        public const string DEMO_BOTH_APP = $"{DEMO_APP}_{DEMO_ADMIN_APP}";
        public const string DEMO_APP_SCOPE = "microservice_demo_app_api";
    }

    public static class SystemRole
    {
        public const string ADMIN = "ADMIN";
        public const string USER = "USER";
    }
}