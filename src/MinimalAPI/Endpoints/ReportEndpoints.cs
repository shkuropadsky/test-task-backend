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

