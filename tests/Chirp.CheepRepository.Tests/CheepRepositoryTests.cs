using System.ComponentModel.DataAnnotations;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.CheepRepository.Tests;

public class CheepRepositoryTests
{
    private readonly ChirpDBContext _dbContext;
    private readonly SqliteConnection _connection;
    public CheepRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(_connection);
        _dbContext = new ChirpDBContext(builder.Options);
        
        _dbContext.Database.EnsureCreated();
    }
    
    [Fact]
    public async Task IsThereACheepRepository()
    {
        // Arrange
         ICheepRepository repository = new Infrastructure.CheepRepository(_dbContext);

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
        PopulateDatabase(_dbContext);
        ICheepRepository repository = new Infrastructure.CheepRepository(_dbContext);
        
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
        await PopulateDatabase(_dbContext);
        ICheepRepository repository = new Infrastructure.CheepRepository(_dbContext);
        
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
        await PopulateDatabase(_dbContext);
        ICheepRepository repository = new Infrastructure.CheepRepository(_dbContext);
        
        // Act
        var cheeps = await repository.GetCheepsFromAuthor("NonExistentAuthor",1);
        
        // Assert
        Assert.Empty(cheeps);
    }
    
    [Fact]
    public async Task CreateCheep()
    {
        // Arrange
        var authorDto = new AuthorDTO { Name = "John Doe", Email = "jndo@itu.dk" };
        var cheepDto = new CheepDTO {Author = authorDto.Name, Text = "I am alive", TimeStamp = DateTime.UtcNow.ToString("MM'/'dd'/'yy H':'mm':'ss") };
        
        ICheepRepository cheepRepository = new Infrastructure.CheepRepository(_dbContext);
    
        // Act
        var createdCheep = await cheepRepository.CreateCheep(cheepDto);
        
        // Assert
        Assert.NotNull(createdCheep);
        Assert.Equal("I am alive", createdCheep.Text);
        Assert.Equal("John Doe", createdCheep.Author.UserName);
        
        var cheeps = await cheepRepository.GetCheeps(1);
        Assert.Single(cheeps); 
        
        var fetchedCheep = cheeps.First();
        Assert.Equal("I am alive", fetchedCheep.Text);
        Assert.Equal("John Doe", fetchedCheep.Author);
    }     
    
    [Fact]
    public async Task CreateCheep_ThrowsException_WhenTextExceeds160Characters()
    {
        // Arrange
        ICheepRepository repository = new Infrastructure.CheepRepository(_dbContext);
        var cheepDto = new CheepDTO
        {
            Text = new string('a', 161),
            Author = "John Doe",
            TimeStamp = "01/02/24 3:04:05",
        };
        var authorDto = new AuthorDTO
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
        };
        
        // Assert
        var exception = Assert.ThrowsAsync<ValidationException>(() => repository.CreateCheep(cheepDto));
        Assert.Equal("Cheep is invalid: Cheeps can't be longer than 160 characters.", exception.Result.Message);
    }

    private async Task PopulateDatabase(ChirpDBContext context)
    {
        var specificDate1 = new DateTime(2024, 1, 2, 3, 4, 5);
        var specificDate2 = new DateTime(2024, 2, 3, 4, 5, 6);
        ICollection<Cheep> authorCheeps1 = new List<Cheep>();
        IAuthorRepository authorRepository = new Infrastructure.AuthorRepository(_dbContext);
        
        
        var authorDTO1 = new AuthorDTO
        {
            Name = "John Doe", 
            Email = "email1",
        };
        var authorDTO2 = new AuthorDTO
        {
            Name = "Mary Doe", 
            Email = "email2",
        };

        var author1 = await authorRepository.CreateAuthor(authorDTO1);
        var author2 = await authorRepository.CreateAuthor(authorDTO2);

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