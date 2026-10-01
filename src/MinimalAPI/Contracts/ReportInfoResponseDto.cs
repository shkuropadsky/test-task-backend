namespace MinimalAPI.Contracts;

public record ReportInfoResponseDto(
    Guid Query,
    int Percent,
    ReportInfoResponseResultDto? Result
);

public record ReportInfoResponseResultDto(
    Guid UserId, 
    string CountSignIn
);