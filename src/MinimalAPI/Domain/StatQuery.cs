namespace MinimalAPI.Domain;

public record StatQuery(
    Guid Id,
    Guid UserId,
    DateTime From,
    DateTime To,
    DateTime CreatedAt
);
