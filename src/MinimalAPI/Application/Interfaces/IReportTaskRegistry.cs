using MinimalAPI.Application.Models;

namespace MinimalAPI.Application.Interfaces;

public interface IReportTaskRegistry
{
    public void Add(Guid taskId, ReportTask task);

    public ReportTask? Get(Guid taskid);
}