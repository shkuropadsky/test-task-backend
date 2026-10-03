using MinimalAPI.Application.Interfaces;
using MinimalAPI.Application.Models;
using MinimalAPI.Domain;

namespace MinimalAPI.Application.Services;

public class ReportManager : IReportManager
{
    private ILogger<ReportManager> _logger;
    private IReportTaskRegistry _taskRegistry;

    public ReportManager(ILogger<ReportManager> logger, IReportTaskRegistry taskRegistry)
    {
        _taskRegistry = taskRegistry;
        _logger = logger;
    }

    public void RegisterDomainTask(StatQueryTask domainTask)
    {
        ReportTask task = new(domainTask);
        _taskRegistry.Add(domainTask.Query.Id, task);

        task.ThreadTask = Task.Run(async () => await domainTask.Generate());
    }

    public void UnRegisterDomainTask(Guid queryId)
    {
        _taskRegistry.Remove(queryId);
    }

    public StatQueryTask? GetDomainTask(Guid queryId)
    {
        var reportTask = _taskRegistry.Get(queryId);
        return reportTask?.DomainTask;
    }

}