using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Application.Interfaces;
using MinimalAPI.Contracts;

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

            Guid response = await reportService.PostReportUserStatisticsAsync(request);

            return response.ToString();
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

            logger.LogDebug("Query={Query}: Percent={Percent}%, Result={Result} ", response.Query, response.Percent, response.Result);
            return Results.Ok(response);
        });
    }
}

