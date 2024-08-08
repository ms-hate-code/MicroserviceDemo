namespace MicroserviceDemo.BuildingBlock.Web;

public static class GlobalExtension
{
    public static string GetCurrentHost()
    {
        var iPAddress = Environment.GetEnvironmentVariable("HOST_PORT_INSTANCE") ??
                        throw new InvalidOperationException("Cannot get current host");
        
        return iPAddress;
    }
}