using System.Text.Json;
using MinimalAPI.Application.Interfaces;
using MinimalAPI.Contracts;
using MinimalAPI.Domain;

namespace MinimalAPI.Application.Services;

public class ReportService : IReportService
{
    private IServiceProvider _serviceProvider;
    private IServiceScopeFactory _scopeFactory;
    private ILogger<ReportService> _logger;
    private IReportManager _manager;
    private IReportStorage _storage;

    public ReportService(IServiceProvider serviceProvider,
                         IServiceScopeFactory scopeFactory,
                         ILogger<ReportService> logger,
                         IReportManager manager,
                         IReportStorage storage)
    {
        _serviceProvider = serviceProvider;
        _scopeFactory = scopeFactory;
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

        await _storage.AddQueryAsync(statQuery);
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
            Query = statQuery,
            Percent = 0,
            Result = null
        };

        domainTask.OnProgress += OnDomainTaskProgress;
        _manager.RegisterDomainTask(domainTask);
    }

    /// <summary>
    /// Сохраняет результаты запроса в базе,
    /// убирает её из реестра активных задач
    /// и отписывается от её событий
    /// </summary>
    private async Task FinishReportTask(StatQueryTask domainTask)
    {
        domainTask.OnProgress -= OnDomainTaskProgress;

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var storage = scope.ServiceProvider.GetRequiredService<IReportStorage>();
            var manager = scope.ServiceProvider.GetRequiredService<IReportManager>();

            await storage.AddQueryResultAsync(domainTask.Query.Id, domainTask.Result!);
            manager.UnRegisterDomainTask(domainTask.Query.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception в FinishReportTask (Query={QueryId}) {exception}", domainTask.Query.Id, ex);
        }
    }

    public async Task<ReportInfoResponseDto?> GetReportInfoAsync(Guid queryId)
    {
        ReportInfoResponseDto response;
        StatQueryTask? queryTask = _manager.GetDomainTask(queryId);

        if (queryTask == null)
        {
            StatQuery? query = await _storage.GetQueryAsync(queryId);
            if (query == null)
            {
                // запрос не найден
                _logger.LogError("Запрос {queryId} не найден!", queryId);
                return null;
            }

            StatQueryResult? queryResult = await _storage.GetQueryResultAsync(queryId);
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

    /// <summary>
    /// Обрабатывает событие от доменной задачи: прогресс подготовки отчёта
    /// </summary>
    private async void OnDomainTaskProgress(object? sender, StatQueryTaskProgressEventArgs e)
    {
        try
        {
            StatQueryTask queryTask = (StatQueryTask)sender!;

            if (e.Percent == 0)
            {
                _logger.LogInformation("================================");
                _logger.LogInformation("НАЧАЛО: Query.Id: {queryId}", queryTask.Query.Id);
                _logger.LogInformation("--------------------------------");
            }
            else if (e.Percent == 100)
            {
                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(queryTask.Result, jsonOptions);

                _logger.LogInformation("--------------------------------");
                _logger.LogInformation("КОНЕЦ: Query.Id: {queryId}", queryTask.Query.Id);
                _logger.LogInformation("         UserId: {UserId}", queryTask.Query.UserId);
                _logger.LogInformation("         Result:\n{Result}", json);
                _logger.LogInformation("================================");

                await FinishReportTask(queryTask);
            }
            else
            {
                _logger.LogInformation("{Percent}% ({Message})", e.Percent, e.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Exception в OnDomainTaskProgress: {exception}", ex);
        }
    }

}