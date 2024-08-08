using MicroserviceDemo.APIGateway.Extensions;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Consul;
using MicroserviceDemo.BuildingBlock.Consul;
using MicroserviceDemo.BuildingBlock.Jwt;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using MicroserviceDemo.BuildingBlock.OpenTelemetry;
using MicroserviceDemo.BuildingBlock.ProblemDetails;
using Ocelot.Cache.CacheManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// specific multiple ocelot file in Routes folder
var routes = "Routes";
builder.Configuration.AddOcelotWithSwaggerSupport(options =>
{
    options.Folder = routes;
});
builder.Services.AddJwt();
//add ocelot with consul
builder.Services
    .AddSwaggerForOcelot(builder.Configuration)
    .AddOcelot(builder.Configuration)
    .AddPolly()
    .AddCacheManager(x =>
    {
        x.WithDictionaryHandle();
    })
    .AddConsul()
    .AddConfigStoredInConsul();

builder.Services.ConfigureOcelotServiceDiscoveryProvider();

var configPath = Path.Combine(builder.Environment.ContentRootPath, routes);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("ocelot.json")
    .AddOcelot(configPath, builder.Environment)
    .AddEnvironmentVariables();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddConsul();

builder.Services.AddProblemDetails();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddCustomOpenTelemetry(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}
app.UseCors();
app.UseRouting();
app.UseCustomProblemDetails();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app
    .UseSwaggerForOcelotUI(options =>
    {
        options.PathToSwaggerGenerator = "/swagger/docs";
    })
    .UseOcelot()
    .Wait();

app.Run();
