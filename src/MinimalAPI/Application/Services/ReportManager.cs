using MinimalAPI.Application.Interfaces;
using MinimalAPI.Application.Models;
using MinimalAPI.Domain;

namespace MinimalAPI.Application.Services;

public class ReportManager : IReportManager
{
    private IReportTaskRegistry _tasks;

    public ReportManager(IReportTaskRegistry tasks)
    {
        _tasks = tasks;
    }

    public Guid RegisterDomainTask(StatQueryTask domainTask)
    {
        Task threadTask = Task.Run(() => domainTask.Generate());

        ReportTask task = new()
        {
            DomainTask = domainTask,
            ThreadTask = threadTask
        };

        _tasks.Add(domainTask.Query.Id, task);
        return domainTask.Query.Id;
    }

    public StatQueryTask? GetDomainTask(Guid queryId)
    {
        var reportTask = _tasks.Get(queryId);
        return reportTask?.DomainTask;
    }

}