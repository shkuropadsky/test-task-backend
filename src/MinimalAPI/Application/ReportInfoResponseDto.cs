namespace MinimalAPI.Application;

public record ReportInfoResponseDto(
    Guid Query, 
    int Percent, 
    StatQueryResultDto? Result
);