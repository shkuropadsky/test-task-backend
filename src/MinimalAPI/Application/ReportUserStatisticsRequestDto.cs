namespace MinimalAPI.Application;

public record ReportUserStatisticsRequestDto(
    Guid UserId,
    DateTime From,
    DateTime To
);

