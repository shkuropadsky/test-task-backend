using MinimalAPI.Domain;

namespace MinimalAPI.Application.Interfaces;

public interface IReportStorage
{
    void AddQuery(StatQuery query);
    StatQuery? GetQuery(Guid queryId);

    void AddQueryResult(Guid queryId, StatQueryResult queryResult);
    StatQueryResult? GetQueryResult(Guid queryId);
}