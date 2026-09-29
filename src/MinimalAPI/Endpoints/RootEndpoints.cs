namespace MinimalAPI.Endpoints;

public static class RootEndpoints
{
    public static void MapRootEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/", (ILogger<Program> logger) =>
        {
            logger.LogInformation("--- Root ---");
            logger.LogDebug("Время: {Time:HH:mm:ss.fff}", DateTime.Now);
            return "Hello World!";
        });
    }
}