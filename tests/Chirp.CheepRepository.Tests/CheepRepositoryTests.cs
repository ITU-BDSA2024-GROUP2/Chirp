using System.Reflection;
using Chirp.Razor;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.CheepRepository.Tests;

public class CheepRepositoryTests
{
    public CheepRepositoryTests()
    {
       /* using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection).Options;

        using var context = new ChirpContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database
        
        ICheepRepository repository = new CheepRepository(context); // Source: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#customize-webapplicationfactory
        */
    }
    [Fact]
    public async Task IsThereACheepRepository()
    {
        using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        using var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database
        
        ICheepRepository repository = new Razor.CheepRepository(context); // Source: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#customize-webapplicationfactory
        
        Assert.NotNull(repository);
    }
}