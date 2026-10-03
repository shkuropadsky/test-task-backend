using MinimalAPI.Application.Models;

namespace MinimalAPI.Application.Interfaces;

public interface IReportTaskRegistry
{
    public void Add(Guid taskId, ReportTask task);

    public void Remove(Guid taskId);

    public ReportTask? Get(Guid taskid);
}