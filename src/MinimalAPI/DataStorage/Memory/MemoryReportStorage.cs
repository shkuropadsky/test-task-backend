using System.Collections.Concurrent;
using MinimalAPI.Application.Interfaces;
using MinimalAPI.Domain;

namespace MinimalAPI.DataStorage.Memory;

public class MemoryReportStorage : IReportStorage
{
    ConcurrentDictionary<Guid, StatQuery> queries = new();
    ConcurrentDictionary<Guid, StatQueryResult> queryResults = new();

    public async Task AddQueryAsync(StatQuery query)
    {
        queries.TryAdd(query.Id, query);
    }

    public async Task AddQueryResultAsync(Guid queryId, StatQueryResult queryResult)
    {
        queryResults.TryAdd(queryId, queryResult);
    }

    public async Task<StatQuery?> GetQueryAsync(Guid queryId)
    {
        queries.TryGetValue(queryId, out var query);
        return query;
    }

    public async Task<StatQueryResult?> GetQueryResultAsync(Guid queryId)
    {
        queryResults.TryGetValue(queryId, out var queryResult);
        return queryResult;
    }
}