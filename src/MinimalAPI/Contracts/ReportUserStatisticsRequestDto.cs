namespace MinimalAPI.Contracts;

public record ReportUserStatisticsRequestDto(
    Guid UserId,
    DateTime From,
    DateTime To
);

