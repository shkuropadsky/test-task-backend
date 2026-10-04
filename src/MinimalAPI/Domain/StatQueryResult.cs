namespace MinimalAPI.Domain;

// для возможности добавления в результат других данных
public record StatQueryResult(Guid Id, Guid StatQueryId, int CountSignIn);