using MinimalAPI.Application.Interfaces;
using MinimalAPI.Application.Models;
using MinimalAPI.Domain;

namespace MinimalAPI.Application.Services;

public class ReportManager : IReportManager
{
    private IReportTaskRegistry _taskRegistry;

    public ReportManager(IReportTaskRegistry taskRegistry)
    {
        _taskRegistry = taskRegistry;
    }

    public Guid RegisterDomainTask(StatQueryTask domainTask)
    {
        Task threadTask = Task.Run(async () => await domainTask.Generate());

        ReportTask task = new()
        {
            DomainTask = domainTask,
            ThreadTask = threadTask
        };

        _taskRegistry.Add(domainTask.Query.Id, task);
        return domainTask.Query.Id;
    }

    public StatQueryTask? GetDomainTask(Guid queryId)
    {
        var reportTask = _taskRegistry.Get(queryId);
        return reportTask?.DomainTask;
    }

}