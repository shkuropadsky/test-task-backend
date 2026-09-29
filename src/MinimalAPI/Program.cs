using MinimalAPI.Endpoints;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpLogging(logging => { });

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

app.UseHttpLogging();
app.UseSerilogRequestLogging(); 

app.Logger.LogInformation("---=== Тестовое задание ===---");
app.Logger.LogDebug("Время: {Time:HH:mm:ss.fff}", DateTime.Now);

if (app.Environment.IsDevelopment())
{
    // {host}/openapi/v1.json
    // {host}/scalar/v1
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.LogInformation("--- Root ---");
    logger.LogDebug("Время: {Time:HH:mm:ss.fff}", DateTime.Now);
    return "Hello World!";
});

app.MapReportEndpoints();

app.Run();


