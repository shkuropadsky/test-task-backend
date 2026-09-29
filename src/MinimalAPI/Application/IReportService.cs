namespace MinimalAPI.Application;

public interface IReportService
{
    Task<ReportInfoResponseDto?> GetReportInfoAsync(Guid query);
}