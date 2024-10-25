using Chirp.Infrastructure;
using JetBrains.Annotations;
using Xunit;
using Chirp.Core;
using Microsoft.Data.Sqlite;

namespace Chirp.AuthorRepository.Tests;

[TestSubject(typeof(Infrastructure.AuthorRepository))]
public class AuthorRepositoryTest
{
    private readonly ChirpDBContext _dbContext;
    private readonly SqliteConnection _connection;
    [Fact]
    public async Task CreateAuthor_Stores_New_Author_In_Database()
    {
        // Arrange
        ICheepRepository repository = new Infrastructure.CheepRepository(_dbContext);
        
        // Act
        AuthorDTO authorDto = new AuthorDTO
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
        };
        var createdAuthor = await repository.CreateAuthor(authorDto);
        var foundAuthor = await repository.FindAuthor(authorDto);

        // Assert
        Assert.NotNull(createdAuthor);
        Assert.Equal(authorDto.Name, createdAuthor.Name);
        Assert.Equal(authorDto.Email, createdAuthor.Email);
        
        Assert.NotNull(foundAuthor);
        Assert.Equal(authorDto.Name, foundAuthor.Name);
        Assert.Equal(authorDto.Email, foundAuthor.Email);
    }

    [Fact]
    public async Task FindAuthor()
    {
        // Arrange
        var authorDto = new AuthorDTO { Name = "John Doe", Email = "email1" };

        await PopulateDatabase(_dbContext);

        ICheepRepository cheepRepository = new Infrastructure.CheepRepository(_dbContext);
    
        // Act
        var author = await cheepRepository.FindAuthor(authorDto);

        // Assert
        Assert.NotNull(author);
        Assert.Equal("John Doe", author.Name);
        Assert.Equal("email1", author.Email);
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
    
    public void Dispose()
    {
        // Dispose of the in-memory SQLite connection
        _dbContext.Dispose();
        _connection.Dispose();
    }
}