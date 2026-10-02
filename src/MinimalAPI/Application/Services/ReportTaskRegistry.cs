using System.Collections.Concurrent;
using MinimalAPI.Application.Interfaces;
using MinimalAPI.Application.Models;
using MinimalAPI.Domain;

namespace MinimalAPI.Application.Services;

public class ReportTaskRegistry : IReportTaskRegistry
{
    /// <summary>
    /// Демо-записи для тестов схемы API (ключи)
    /// </summary>
    public const string DEMO_PROGRESS_077 = "00000077-e090-4d18-8654-678e463b7aaa";
    public const string DEMO_PROGRESS_100 = "00000100-e090-4d18-8654-678e463b7bbb";
    public const string DEMO_USER = "b28d0ced-8af5-4c94-8650-c7946241fd1a";

    private readonly ConcurrentDictionary<Guid, ReportTask> _tasks;

    private IServiceProvider _serviceProvider;
    private ILogger<ReportService> _logger;

    public ReportTaskRegistry(IServiceProvider serviceProvider, ILogger<ReportService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        _tasks = new ConcurrentDictionary<Guid, ReportTask>();

        // Демо-записи для тестов схемы API (объекты-заглушки)
        AddDemoRecords();
    }

    public void Add(Guid id, ReportTask task)
    {
        _tasks.TryAdd(id, task);
    }

    public ReportTask? Get(Guid taskId)
    {
        _tasks.TryGetValue(taskId, out var reportTask);
        return reportTask;
    }

    /// <summary>
    /// Добавляет в реестр демо-записи для тестирования только API:
    /// только данные, без реальных фоновых задач 
    /// </summary>
    private void AddDemoRecords()
    {
        StatQuery query77 = new(Guid.Parse(DEMO_PROGRESS_077), Guid.Parse(DEMO_USER), DateTime.Now, DateTime.Now);
        _tasks.TryAdd(query77.Id, new ReportTask()
        {
            DomainTask = new StatQueryTask(_serviceProvider)
            {
                Query = query77,
                Percent = 77
            },
            ThreadTask = new Task(() => { })
        });

        StatQuery query100 = new(Guid.Parse(DEMO_PROGRESS_100), Guid.Parse(DEMO_USER), DateTime.Now, DateTime.Now);
        _tasks.TryAdd(query100.Id, new ReportTask()
        {
            DomainTask = new StatQueryTask(_serviceProvider)
            {
                Query = query100,
                Percent = 100,
                Result = new StatQueryResult(StatQueryTask.DEFAULT_COUNT_SIGN_IN)
            },
            ThreadTask = new Task(() => { })
        });
    }

}