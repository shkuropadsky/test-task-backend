using Microsoft.EntityFrameworkCore;
using MinimalAPI.Application.Interfaces;
using MinimalAPI.Domain;

namespace MinimalAPI.DataStorage.Postgres;

public class PostgresReportStorage : IReportStorage
{
    private readonly BaseReportDbContext _context;

    public PostgresReportStorage(BaseReportDbContext context)
    {
        _context = context;
    }

    public async Task AddQueryAsync(StatQuery query)
    {
        await _context.Queries.AddAsync(query);
        await _context.SaveChangesAsync();
    }

    public async Task<StatQuery?> GetQueryAsync(Guid queryId)
    {
        return await _context.Queries.FindAsync(queryId);
    }

    public async Task AddQueryResultAsync(Guid queryId, StatQueryResult queryResult)
    {
        // Перед вставкой гарантируем целостность данных: 
        // Привязываем результат к правильному Id запроса через синтаксис record 'with'
        var finalizedResult = queryResult with { StatQueryId = queryId };

        await _context.QueryResults.AddAsync(finalizedResult);
        await _context.SaveChangesAsync();
    }

    public async Task<StatQueryResult?> GetQueryResultAsync(Guid queryId)
    {
        // Ищем в таблице результатов запись, у которой внешний ключ равен queryId
        return await _context.QueryResults
            .FirstOrDefaultAsync(r => r.StatQueryId == queryId);
    }
}
