using Microsoft.Extensions.Options;

namespace MinimalAPI.Domain;

public class StatQueryTask(IServiceProvider serviceProvider)
{
    public required StatQuery Query { get; init; }

    public int Percent { get; set; }

    public StatQueryResult? Result { get; set; }

    public async Task Generate()
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<StatQueryTask>>();

        var settingsOptions = scope.ServiceProvider.GetRequiredService<IOptions<StatQueryTaskSettings>>();
        int delayInMs = settingsOptions.Value.DelayInMilliSeconds;
        int stepsNumber = 100;
        double stepDelay = (double)delayInMs / stepsNumber;

        DateTime t1 = DateTime.Now;
        logger.LogDebug("Начало: {Time:HH:mm:ss.fff}", t1);
        logger.LogDebug("Query: {QueryId}", Query.Id);

        logger.LogDebug("================================");
        logger.LogDebug("Generate: {Percent} %", Percent);

        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(stepDelay));

        Percent = 0;
        while (Percent < stepsNumber)
        {
            await timer.WaitForNextTickAsync();
            Percent++;
            logger.LogDebug("Generate: {Percent} %", Percent);
        }

        Result = new(CountSignIn: 12);

        logger.LogDebug("--------------------------------");
        logger.LogDebug("Generate: {Percent} %", Percent);
        logger.LogDebug(" QueryId: {QueryId}", Query.Id);
        logger.LogDebug("  UserId: {UserId}", Query.UserId);
        logger.LogDebug("          {Result}", Result);
        logger.LogDebug("================================");

        DateTime t2 = DateTime.Now;
        logger.LogDebug("Конец: {Time:HH:mm:ss.fff}", t2);
        TimeSpan span = t2 - t1;
        logger.LogDebug("Время: {Span:hh\\:mm\\:ss}", span);


    }

}