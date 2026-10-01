namespace MinimalAPI.Domain;

public class StatQueryTask
{
    public required StatQuery Query { get; init; }

    public int Percent { get; set; }

    public StatQueryResult? Result { get; set; }

    public void Generate()
    {
        Console.WriteLine("GENERATE");
        Percent = 100;
        Result = new(CountSignIn: 12);
    }

}