using MicroserviceDemo.BuildingBlock.Consul;
using MicroserviceDemo.BuildingBlock.OpenTelemetry;
using MicroserviceDemo.BuildingBlock.MassTransit;
using System.Reflection;
using Grpc.AspNetCore.Server;
using MicroserviceDemo.BuildingBlock.Caching;
using MicroserviceDemo.BuildingBlock.gRPC;
using MicroserviceDemo.BuildingBlock.ProblemDetails;
using MicroserviceDemo.ServiceB.Application.Services.gRPC;
using MicroserviceDemo.ServiceB.Infrastructure.Services.HostedService;

var builder = WebApplication.CreateBuilder(args);
builder.UseCustomKestrelGrpc();
// Add services to the container.
builder.Configuration
    .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddCustomRedisCaching();
builder.Services.AddSwaggerGen();
builder.Services.AddCustomOpenTelemetry(builder.Configuration);
builder.Services.AddCustomMassTransit(builder.Configuration);
builder.Services.AddHostedService<HostRegistrationService>();
builder.Services.AddGrpc();

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

app.MapGrpcService<TestBService>();

app.MapControllers();
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapGet("/api/testB/exception", x => throw new Exception());
app.MapGet("/api/healths/status", x => x.Response.WriteAsync("Ok!!!"));

app.Run();
