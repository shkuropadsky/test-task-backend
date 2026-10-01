using MinimalAPI.Contracts;

namespace MinimalAPI.Application.Interfaces;

public interface IReportService
{
    Task<Guid> PostReportUserStatisticsAsync(ReportUserStatisticsRequestDto request);

    Task<ReportInfoResponseDto?> GetReportInfoAsync(Guid query);
}