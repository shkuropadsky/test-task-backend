using System.Collections.Concurrent;
using MinimalAPI.Application.Interfaces;
using MinimalAPI.Application.Models;

namespace MinimalAPI.Application.Services;

public class ReportTaskRegistry : IReportTaskRegistry
{
    private readonly ConcurrentDictionary<Guid, ReportTask> _tasks;

    private IServiceProvider _serviceProvider;
    private ILogger<ReportService> _logger;

    public ReportTaskRegistry(IServiceProvider serviceProvider, ILogger<ReportService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        _tasks = new ConcurrentDictionary<Guid, ReportTask>();
    }

    public void Add(Guid id, ReportTask task)
    {
        _tasks.TryAdd(id, task);
    }

    public void Remove(Guid id)
    {
        _tasks.TryRemove(id, out _);
    }

    public ReportTask? Get(Guid taskId)
    {
        _tasks.TryGetValue(taskId, out var reportTask);
        return reportTask;
    }

}