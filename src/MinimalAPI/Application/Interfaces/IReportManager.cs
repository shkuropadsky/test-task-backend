using MinimalAPI.Domain;

namespace MinimalAPI.Application.Interfaces;

public interface IReportManager
{
    Guid RegisterDomainTask(StatQueryTask domainTask);

    StatQueryTask? GetDomainTask(Guid queryId);

}