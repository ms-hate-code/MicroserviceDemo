using MicroserviceDemo.BuildingBlock.Consul;
using MicroserviceDemo.BuildingBlock.MassTransit;
using MicroserviceDemo.BuildingBlock.OpenTelemetry;
using MicroserviceDemo.BuildingBlock.ProblemDetails;
using MicroserviceDemo.LoadBalancer.Application.Services.CustomProxyConfig;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration
    .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddSingleton<IProxyConfigProvider>(x => new CustomProxyConfigProvider(builder.Configuration))
    .AddReverseProxy();

builder.Services
    .AddSingleton<ICustomProxyConfigService, CustomProxyConfigService>();
builder.Services.AddCustomOpenTelemetry(builder.Configuration);
builder.Services.AddConsul();
builder.Services.AddProblemDetails();
builder.Services.AddCustomMassTransit(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomProblemDetails();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapReverseProxy();
});
app.MapGet("/api/healths/status", x => x.Response.WriteAsync("Ok!!!"));
app.Run();
