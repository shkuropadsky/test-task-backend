using MinimalAPI.Endpoints;
using MinimalAPI.Application;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddHttpLogging(logging => { });
builder.Services.AddOpenApi();

builder.Services.AddScoped<IReportService, ReportService>();

var app = builder.Build();

Log.Information("---=== Тестовое задание ===---");
Log.Debug("Время: {Time:HH:mm:ss.fff}", DateTime.Now);

app.UseHttpLogging();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    // {host}/openapi/v1.json
    // {host}/scalar/v1
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapRootEndpoints();
app.MapReportEndpoints();

app.Run();


