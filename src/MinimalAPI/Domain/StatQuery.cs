namespace MinimalAPI.Domain;

public class StatQuery
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public DateTime From { get; init; }
    public DateTime To { get; init; }

    public StatQueryResult? Result { get; set; }
}