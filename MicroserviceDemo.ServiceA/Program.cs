using Grpc.Net.Client;
using MicroserviceDemo.BuildingBlock.Consul;
using MicroserviceDemo.BuildingBlock.OpenTelemetry;
using MicroserviceDemo.BuildingBlock.RestEase;
using MicroserviceDemo.BuildingBlock.MassTransit;
using MicroserviceDemo.ServiceA.Infrastructure.Service;
using MicroserviceDemo.BuildingBlock.Caching;
using MicroserviceDemo.BuildingBlock.Jwt;
using MicroserviceDemo.BuildingBlock.ProblemDetails;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration
    .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwt();
builder.Services.AddProblemDetails();
builder.Services.AddGrpcClient<TestBProtoService.Generated.TestBProtoService.TestBProtoServiceClient>
    (o =>
    {
        o.Address = new Uri("https://localhost:5270");
        o.ChannelOptionsActions.Add(x =>
        {
            x.HttpHandler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
        });
    });

builder.Services.AddCustomRedisCaching();
builder.Services.AddScoped<ILoadBalancingService, LoadBalancingService>();
builder.Services.AddHostedService<HostRegistrationService>();
builder.Services.AddCustomOpenTelemetry(builder.Configuration);
builder.Services.RegisterServiceForwarder<IServiceBClientAPI>("serviceB");
builder.Services.AddCustomMassTransit(builder.Configuration);
builder.Services.AddConsul();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomProblemDetails();

app.UseAuthorization();

app.MapControllers();
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapGet("/api/testA/exception", x => throw new Exception());
app.MapGet("/api/healths/status", x => x.Response.WriteAsync("Ok!!!"));

app.Run();
