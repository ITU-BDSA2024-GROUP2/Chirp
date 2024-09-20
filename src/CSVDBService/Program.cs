using Chirp.CLI;
using CSVDB;

var database = CSVDatabase<Cheep>.Instance;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello world!");
//app.MapPost("/cheep", (Cheep cheep) => database.Store(cheep));

app.Run();