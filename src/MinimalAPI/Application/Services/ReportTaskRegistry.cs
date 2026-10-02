using System.Collections.Concurrent;
using MinimalAPI.Application.Interfaces;
using MinimalAPI.Application.Models;
using MinimalAPI.Domain;

namespace MinimalAPI.Application.Services;

public class ReportTaskRegistry : IReportTaskRegistry
{
    // Демо-записи для тестов схемы API (ключи)
    public const string DEMO_PROGRESS_077 = "00000077-e090-4d18-8654-678e463b7aaa";
    public const string DEMO_PROGRESS_100 = "00000100-e090-4d18-8654-678e463b7bbb";

    private readonly ConcurrentDictionary<Guid, ReportTask> _tasks;

    public ReportTaskRegistry()
    {
        _tasks = new ConcurrentDictionary<Guid, ReportTask>();

        // Демо-записи для тестов схемы API (объекты-заглушки)
        StatQuery query77 = new(Id: Guid.Parse(DEMO_PROGRESS_077), Guid.Empty, DateTime.Now, DateTime.Now);
        _tasks.TryAdd(query77.Id, new ReportTask()
        {
            DomainTask = new StatQueryTask(null) { Query = query77, Percent = 77 },
            ThreadTask = null
        });
        StatQuery query100 = new(Id: Guid.Parse(DEMO_PROGRESS_100), Guid.Empty, DateTime.Now, DateTime.Now);
        _tasks.TryAdd(query100.Id, new ReportTask()
        {
            DomainTask = new StatQueryTask(null) { Query = query100, Percent = 100, Result = new StatQueryResult(42) },
            ThreadTask = null
        });
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
}