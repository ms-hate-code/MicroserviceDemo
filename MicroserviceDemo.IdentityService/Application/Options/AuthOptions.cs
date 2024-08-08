namespace MicroserviceDemo.IdentityService.Application.Options;

public class AuthOptions
{
    public string IssuerUri { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Scope { get; set; }
}