using System.Collections.Concurrent;
using MinimalAPI.Application.Interfaces;
using MinimalAPI.Application.Models;

namespace MinimalAPI.Application.Services;

public class ReportTaskRegistry : IReportTaskRegistry
{
    private readonly ConcurrentDictionary<Guid, ReportTask> _tasks;

    public ReportTaskRegistry()
    {
        _tasks = new ConcurrentDictionary<Guid, ReportTask>();
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