using MinimalAPI.Application.Interfaces;
using MinimalAPI.Contracts;
using MinimalAPI.Domain;

namespace MinimalAPI.Application.Services;

public class ReportService : IReportService
{
    private IServiceProvider _serviceProvider;
    private ILogger<ReportService> _logger;
    private IReportManager _manager;
    private IReportStorage _storage;

    public ReportService(IServiceProvider serviceProvider,
                         ILogger<ReportService> logger,
                         IReportManager manager,
                         IReportStorage storage)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _manager = manager;
        _storage = storage;
    }

    public async Task<Guid> PostReportUserStatisticsAsync(ReportUserStatisticsRequestDto request)
    {
        StatQuery statQuery = new(
            Id: Guid.NewGuid(),
            UserId: request.UserId,
            From: request.From,
            To: request.To
        );

        _logger.LogDebug("Новый запрос: {queryId}", statQuery.Id);

        _storage.AddQuery(statQuery);
        StartReportTask(statQuery);

        return statQuery.Id;
    }

    /// <summary>
    /// Создаёт для запроса новую runtime-задачу, 
    /// передаёт её для регистрации и запуска
    /// и подписывается на прогресс задачи
    /// </summary>
    private void StartReportTask(StatQuery statQuery)
    {
        // runtime-задача для запроса
        StatQueryTask domainTask = new(_serviceProvider)
        {
            Query = statQuery
            Query = statQuery,
            Percent = 0,
            Result = null
        };

        _manager.RegisterDomainTask(domainTask);

        return statQuery.Id;
    }


    public async Task<ReportInfoResponseDto?> GetReportInfoAsync(Guid queryId)
    {
        ReportInfoResponseDto response;
        StatQueryTask? queryTask = _manager.GetDomainTask(queryId);

        if (queryTask == null)
        {
            StatQuery query = _storage.GetQuery(queryId);
            if (query == null)
            {
                // запрос не найден
                _logger.LogError("Запрос {queryId} не найден!", queryId);
                return null;
            }

            StatQueryResult queryResult = _storage.GetQueryResult(queryId);
            if (queryResult != null)
            {
                // найден завершённый запрос с результатом
                response = ComposeCompleteResponse(query, queryResult);
            }
            else
            {
                // запуск новой runtime-задачи для имеющегося запроса
                StartReportTask(query);
                response = ComposeInProgressResponse(query, 0);
            }
            return response;
        }

        if (queryTask.Result == null)
        {
            // частично выполненный запрос 
            response = ComposeInProgressResponse(queryTask.Query, queryTask.Percent);
        }
        else
        {
            // полностью выполненный запрос с результатом
            response = ComposeCompleteResponse(queryTask.Query, queryTask.Result);
        }
        return response;
    }

    private ReportInfoResponseDto ComposeInProgressResponse(StatQuery query, int percent)
    {
        ReportInfoResponseDto response = new(
            Query: query.Id,
            Percent: percent,
            Result: null
        );
        return response;
    }

    private ReportInfoResponseDto ComposeCompleteResponse(StatQuery query, StatQueryResult result)
    {
        ReportInfoResponseDto response = new(
            Query: query.Id,
            Percent: 100,
            Result: new(
                UserId: query.UserId,
                // по спецификации CountSignIn в API - это string
                CountSignIn: result.CountSignIn.ToString()
            )
        );
        return response;
    }

}