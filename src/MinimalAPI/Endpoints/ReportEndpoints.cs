using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Application;

namespace MinimalAPI.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this IEndpointRouteBuilder builder)
    {
        var loggerFactory = builder.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(typeof(ReportEndpoints));

        var group = builder.MapGroup("/report");

        group.MapPost("/user_statistics", async (ReportUserStatisticsRequestDto request, IReportService reportService) =>
        {
            logger.LogInformation("--- /report/user_statistics ---");
            logger.LogDebug("Время: {Time:HH:mm:ss.fff}", DateTime.Now);

            string response = "1a98b57d-e090-4d18-8654-678e463b73e8";

            return response;
        });

        group.MapGet("/info", async ([FromQuery] Guid query, IReportService reportService) =>
        {
            logger.LogInformation("--- /report/info ---");
            logger.LogDebug("Время: {Time:HH:mm:ss.fff}", DateTime.Now);

            ReportInfoResponseDto? response = await reportService.GetReportInfoAsync(query);

            if (response == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(response);
        });
    }
}

