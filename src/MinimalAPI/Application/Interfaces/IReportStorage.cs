using MinimalAPI.Domain;

namespace MinimalAPI.Application.Interfaces;

public interface IReportStorage
{
    void AddQuery(StatQuery statQuery);
    StatQuery GetQuery(Guid queryId);
    void AddQueryResult(StatQueryResult? result);
    StatQueryResult GetQueryResult(Guid queryId);
}