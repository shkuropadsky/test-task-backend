using MinimalAPI.Domain;

namespace MinimalAPI.Application.Models;

public class ReportTask
{
    public StatQueryTask DomainTask { get; init; }

    public Task? ThreadTask { get; set; }

    public ReportTask(StatQueryTask domainTask)
    {
        DomainTask = domainTask;
    }
}