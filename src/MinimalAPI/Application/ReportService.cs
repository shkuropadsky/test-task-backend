using MinimalAPI.Domain;

namespace MinimalAPI.Application;

public class ReportService : IReportService
{
    public async Task<ReportInfoResponseDto?> GetReportInfoAsync(Guid query)
    {
        #region test - временная заглушка для тестирования API

        // общий тестовый объект запроса
        StatQuery statQuery = new()
        {
            Id = query,
            UserId = Guid.Parse("b28d0ced-8af5-4c94-8650-c7946241fd1a"),
            From = DateTime.Parse("2026-09-30"),
            To = DateTime.Parse("2026-10-01"),
        };

        if (query == Guid.Parse("1a98b57d-e090-4d18-8654-678e463b7aaa"))
        {
            // частично выполненный запрос 
            // маппинг
            ReportInfoResponseDto response = new(
                Query: statQuery.Id,
                Percent: 77,
                Result: null
            );

            return response;
        }
        else if (query == Guid.Parse("1a98b57d-e090-4d18-8654-678e463b7bbb"))
        {
            // полностью выполненный запрос с результатом
            statQuery.Result = new(12);

            // маппинг
            // по спецификации CountSignIn в API - это string
            StatQueryResultDto resultDto = new(
                UserId: statQuery.UserId,
                CountSignIn: statQuery.Result.CountSignIn.ToString()
            );

            ReportInfoResponseDto response = new(
                Query: statQuery.Id,
                Percent: 100,
                Result: resultDto
            );

            return response;
        }

        #endregion

        // запрос не найден
        return null;
    }
}