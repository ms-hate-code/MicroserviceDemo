using System.Reflection;
using MicroserviceDemo.BuildingBlock.Web;
using MicroserviceDemo.IdentityService.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddMinimalEndpoints(assemblies: Assembly.GetExecutingAssembly());
builder.AddInfrastructure();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseInfrastructure();
app.MapMinimalEndpoints();

app.Run();