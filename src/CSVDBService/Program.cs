using Chirp.CLI;
using CSVDB;
    
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/cheep", (Cheep cheep) => "Hello World!");

app.Run();
