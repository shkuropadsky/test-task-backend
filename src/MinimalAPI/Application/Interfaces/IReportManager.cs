using MinimalAPI.Domain;

namespace MinimalAPI.Application.Interfaces;

public interface IReportManager
{
    void RegisterDomainTask(StatQueryTask domainTask);

    void UnRegisterDomainTask(Guid queryId);

    StatQueryTask? GetDomainTask(Guid queryId);

}