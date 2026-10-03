using System.Collections.Concurrent;
using MinimalAPI.Application.Interfaces;
using MinimalAPI.Domain;

namespace MinimalAPI.DataStorage;

public class ReportStorageMemory : IReportStorage
{
    ConcurrentDictionary<Guid, StatQuery> queries = new();
    ConcurrentDictionary<Guid, StatQueryResult> queryResults = new();

    public void AddQuery(StatQuery query)
    {
        queries.TryAdd(query.Id, query);
    }

    public void AddQueryResult(Guid queryId, StatQueryResult queryResult)
    {
        queryResults.TryAdd(queryId, queryResult);
    }

    public StatQuery? GetQuery(Guid queryId)
    {
        queries.TryGetValue(queryId, out var query);
        return query;
    }

    public StatQueryResult? GetQueryResult(Guid queryId)
    {
        queryResults.TryGetValue(queryId, out var queryResult);
        return queryResult;
    }
}