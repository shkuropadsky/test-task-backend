namespace MinimalAPI.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/report");

        group.MapGet("/info", (ILogger<Program> logger) =>
        {
            logger.LogInformation("--- /report/info ---");
            logger.LogDebug("Время: {Time:HH:mm:ss.fff}", DateTime.Now);
            return "Report: Info";
        });
    }
}