using MinimalAPI.Application.Interfaces;
using MinimalAPI.Contracts;
using MinimalAPI.Domain;

namespace MinimalAPI.Application.Services;

public class ReportService : IReportService
{
    private IReportManager _manager;
    public ReportService(IReportManager manager)
    {
        _manager = manager;
    }

    public async Task<Guid> PostReportUserStatisticsAsync(ReportUserStatisticsRequestDto request)
    {
        StatQuery statQuery = new(
            Id: Guid.NewGuid(),
            UserId: request.UserId,
            From: request.From,
            To: request.To
        );

        StatQueryTask domainTask = new()
        {
            Query = statQuery
        };

        _manager.RegisterDomainTask(domainTask);

        return statQuery.Id;
    }


    public async Task<ReportInfoResponseDto?> GetReportInfoAsync(Guid queryId)
    {
        StatQueryTask? queryTask = _manager.GetDomainTask(queryId);

        if (queryTask == null)
        {
            // запрос не найден
            return null;
        }

        ReportInfoResponseDto response;
        if (queryTask.Result == null)
        {
            // частично выполненный запрос 
            response = new(
                Query: queryId,
                Percent: queryTask.Percent,
                Result: null
            );
        }
        else
        {
            // полностью выполненный запрос с результатом
            response = new(
                Query: queryId,
                Percent: queryTask.Percent,
                Result: new(
                    UserId: queryTask.Query.UserId,
                    // по спецификации CountSignIn в API - это string
                    CountSignIn: queryTask.Result.CountSignIn.ToString()
                )
            );
        }
        return response;
    }
}