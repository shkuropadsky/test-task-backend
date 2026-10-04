using MinimalAPI.Domain;

namespace MinimalAPI.Application.Interfaces;

public interface IReportStorage
{
    Task AddQueryAsync(StatQuery query);
    Task<StatQuery?> GetQueryAsync(Guid queryId);

    Task AddQueryResultAsync(Guid queryId, StatQueryResult queryResult);
    Task<StatQueryResult?> GetQueryResultAsync(Guid queryId);
}