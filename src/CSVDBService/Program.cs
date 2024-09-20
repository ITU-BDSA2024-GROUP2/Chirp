using Chirp.CLI;
using CSVDB;

var database = CSVDatabase<Cheep>.Instance;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

//app.MapGet("/", () => database.Read());

app.MapPost("/", (Cheep cheep) => database.Store(cheep));

app.Run();