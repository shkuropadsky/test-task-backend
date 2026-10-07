using Microsoft.Extensions.Options;

namespace MinimalAPI.Domain;

public class StatQueryTask(IServiceProvider serviceProvider)
{
    public const int DEFAULT_COUNT_SIGN_IN = 12;

    public required StatQuery Query { get; init; }

    public int Percent { get; set; }

    public StatQueryResult? Result { get; set; }

    public event EventHandler<StatQueryTaskProgressEventArgs>? OnProgress;

    public async Task Generate()
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<StatQueryTask>>();

        var settingsOptions = scope.ServiceProvider.GetRequiredService<IOptions<StatQueryTaskSettings>>();
        int delayInMs = settingsOptions.Value.DelayInMilliSeconds;
        int stepsNumber = 100;
        double stepDelay = (double)delayInMs / stepsNumber;

        OnProgress?.Invoke(this, new StatQueryTaskProgressEventArgs(0, "Формирование отчёта начато"));

        DateTime t1 = DateTime.Now;
        logger.LogDebug("Начало: {Time:HH:mm:ss.fff}", t1);
        logger.LogDebug("Query: {QueryId}", Query.Id);

        logger.LogDebug("================================");

        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(stepDelay));

        Percent = 0;
        while (Percent < stepsNumber)
        {
            await timer.WaitForNextTickAsync();
            Percent++;

            if (Percent == 100)
            {
                // новый результат, связанный с исходным запросом по Id
                Result = new(
                    Id: Guid.NewGuid(),
                    StatQueryId: Query.Id,
                    CountSignIn: DEFAULT_COUNT_SIGN_IN,
                    CreatedAt: DateTime.UtcNow);
            }
            else
            {
                OnProgress?.Invoke(this, new StatQueryTaskProgressEventArgs(Percent, "Идёт формирование отчёта..."));
            }
        }

        OnProgress?.Invoke(this, new StatQueryTaskProgressEventArgs(100, "Формирование отчёта завершено"));

        DateTime t2 = DateTime.Now;
        logger.LogDebug("Конец: {Time:HH:mm:ss.fff}", t2);
        TimeSpan span = t2 - t1;
        logger.LogDebug("Время: {Span:hh\\:mm\\:ss}", span);
    }

}