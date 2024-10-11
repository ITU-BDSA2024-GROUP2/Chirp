using Chirp.Razor;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.CheepRepository.Tests;

public class CheepRepositoryTests
{
    [Fact]
    public async Task IsThereACheepRepository()
    {
        // Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        await using var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database
         ICheepRepository repository = new Razor.CheepRepository(context); // Source: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#customize-webapplicationfactory

        // Act
        var cheeps = await repository.GetCheeps(1);
        
        // Assert
        Assert.Empty(cheeps);
        Assert.NotNull(cheeps);
    }

    [Fact]
    public async Task ReadCheepsFromCheepRepositoryTest()
    {
        // Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        await using var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database
        await PopulateDatabase(context);
        ICheepRepository repository = new Razor.CheepRepository(context); // Source: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#customize-webapplicationfactory
        
        // Act
        var cheeps = await repository.GetCheeps(1);
        
        // Assert
        Assert.NotEmpty(cheeps);
        Assert.Equal(2,cheeps.Count());
        
        Assert.Equal("John Doe", cheeps[1].Author);
        Assert.Equal("I am alive", cheeps[1].Text);
        Assert.Equal("01/02/24 3:04:05", cheeps[1].TimeStamp);
        
        Assert.Equal("Mary Doe", cheeps[0].Author);
        Assert.Equal("I am also here", cheeps[0].Text);
        Assert.Equal("02/03/24 4:05:06", cheeps[0].TimeStamp);
        
    }

    [Theory]
    [InlineData("John Doe", "I am alive")]
    [InlineData("Mary Doe", "I am also here")]
    public async Task ReadCheepsFromAuthor_Returns_Cheeps_For_Existing_Author(string author, string expectedText)
    {
        // Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        await using var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database
        await PopulateDatabase(context);
        ICheepRepository repository = new Razor.CheepRepository(context); // Source: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#customize-webapplicationfactory
        
        // Act
        var cheeps = await repository.GetCheepsFromAuthor(author,1);
        
        // Assert
        Assert.NotNull(cheeps);
        Assert.NotEmpty(cheeps);
        
        Assert.Equal(expectedText, cheeps[0].Text);
    }

    [Fact]
    public async Task ReadCheepsFromAuthor_Returns_Empty_For_NonExistent_Author()
    {
        // Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        await using var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database
        await PopulateDatabase(context);
        ICheepRepository repository = new Razor.CheepRepository(context); // Source: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#customize-webapplicationfactory
        
        // Act
        var cheeps = await repository.GetCheepsFromAuthor("NonExistentAuthor",1);
        
        // Assert
        Assert.Empty(cheeps);
    }

    private async Task PopulateDatabase(ChirpDBContext context)
    {
        var specificDate1 = new DateTime(2024, 1, 2, 3, 4, 5);
        var specificDate2 = new DateTime(2024, 2, 3, 4, 5, 6);
        ICollection<Cheep> authorCheeps1 = new List<Cheep>();
        ICollection<Cheep> authorCheeps2 = new List<Cheep>();
        
        var author1 = new Author
        {
            AuthorId = 10, 
            Name = "John Doe", 
            Email = "email1",
            Cheeps = authorCheeps1
        };
        var author2 = new Author
        {
            AuthorId = 20, 
            Name = "Mary Doe", 
            Email = "email2",
            Cheeps = authorCheeps2
        };
        var cheep1 = new Cheep { 
            CheepId = 1, 
            Text = "I am alive", 
            TimeStamp = specificDate1, 
            Author = author1
        };
        var cheep2 = new Cheep { 
            CheepId = 2, 
            Text = "I am also here", 
            TimeStamp = specificDate2, 
            Author = author2
        };
        
        await context.Cheeps.AddAsync(cheep1);
        await context.Cheeps.AddAsync(cheep2);
        await context.SaveChangesAsync();
    }
}