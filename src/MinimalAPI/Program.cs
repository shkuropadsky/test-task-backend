using MinimalAPI.Endpoints;
using Scalar.AspNetCore;
using Serilog;
using System.Text.Json;
using Microsoft.AspNetCore.HttpLogging;
using MinimalAPI.Application.Interfaces;
using MinimalAPI.Application.Services;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestBody 
                          | HttpLoggingFields.RequestQuery
                          | HttpLoggingFields.ResponseBody;
    logging.RequestBodyLogLimit = 4096;
});

builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});


builder.Services.AddSingleton<IReportTaskRegistry, ReportTaskRegistry>();
builder.Services.AddScoped<IReportManager, ReportManager>();
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


