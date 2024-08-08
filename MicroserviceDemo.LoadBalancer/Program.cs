using MicroserviceDemo.BuildingBlock.Consul;
using MicroserviceDemo.BuildingBlock.OpenTelemetry;
using MicroserviceDemo.BuildingBlock.ProblemDetails;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration
    .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddCustomOpenTelemetry(builder.Configuration);
builder.Services.AddConsul();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomProblemDetails();

app.MapReverseProxy();

app.UseAuthorization();

app.MapControllers();
app.MapGet("/api/healths/status", x => x.Response.WriteAsync("Ok!!!"));

app.Run();
