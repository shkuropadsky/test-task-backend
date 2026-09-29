namespace MinimalAPI.Application;

public class ReportService : IReportService
{
    public async Task<ReportInfoResponseDto?> GetReportInfoAsync(Guid query)
    {
        #region test

        if (query == Guid.Parse("1a98b57d-e090-4d18-8654-678e463b7aaa"))
        {
            return null;
        }

        ReportInfoResponseDto response = new(query, 77, null);

        #endregion

        return response;
    }
}