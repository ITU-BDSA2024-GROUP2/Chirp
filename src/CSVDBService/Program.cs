using Chirp.CLI;
using CSVDB;

public class Program
{
    private static IDatabaseRepository<Cheep> database = CSVDatabase<Cheep>.Instance;
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.MapGet("/cheeps", () =>
        {
            return database.Read();
        });

        app.MapPost("/cheep", (Cheep cheep) =>
        {
            database.Store(cheep);
        });

        app.Run();
    }
}
