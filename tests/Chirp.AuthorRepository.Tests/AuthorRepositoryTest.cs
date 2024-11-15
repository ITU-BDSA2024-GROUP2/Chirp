using Chirp.Infrastructure;
using JetBrains.Annotations;
using Xunit;
using Chirp.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.AuthorRepository.Tests;

[TestSubject(typeof(Infrastructure.AuthorRepository))]
public class AuthorRepositoryTest
{
    private readonly ChirpDBContext _dbContext;
    

    public AuthorRepositoryTest()
    {
        SqliteConnection connection;
        connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);
        _dbContext = new ChirpDBContext(builder.Options);

        _dbContext.Database.EnsureCreated();
    }

    [Fact]
    public async Task CreateAuthor_Stores_New_Author_In_Database()
    {
        // Arrange
        var authorDto = new AuthorDTO { Name = "John Doe", Email = "john.doe@example.com" };

        ICheepRepository cheepRepository = new CheepRepository(_dbContext);
        IAuthorRepository authorRepository = new Infrastructure.AuthorRepository(_dbContext);

        // Act
        var createdAuthor = await authorRepository.CreateAuthor(authorDto);
        var author = await cheepRepository.FindAuthorByName(createdAuthor.UserName!);

        // Assert
        Assert.NotNull(createdAuthor);
        Assert.Equal(authorDto.Name, createdAuthor.UserName);
        Assert.Equal(authorDto.Email, createdAuthor.Email);

        Assert.NotNull(author);
        Assert.Equal(authorDto.Name, author.UserName);
        Assert.Equal(authorDto.Email, author.Email);
    }

    [Fact]
    public async Task FindAuthor()
    {
        // Arrange
        var authorDto = new AuthorDTO { Name = "John Doe", Email = "email1" };

        ICheepRepository cheepRepository = new CheepRepository(_dbContext);
        IAuthorRepository authorRepository = new Infrastructure.AuthorRepository(_dbContext);

        // Act
        var createdAuthor = await authorRepository.CreateAuthor(authorDto);
        var author = await cheepRepository.FindAuthorByName(createdAuthor.UserName!);

        // Assert
        Assert.NotNull(author);
        Assert.Equal("John Doe", author.UserName);
        Assert.Equal("email1", author.Email);
    }

    [Fact]
    public async Task IsFollowing()
    {
        // Arrange
        var authorDto = new AuthorDTO { Name = "John Doe", Email = "email1" };
        var authorDto2 = new AuthorDTO { Name = "Not John Doe", Email = "gmail" };
        IAuthorRepository authorRepository = new Infrastructure.AuthorRepository(_dbContext);
        
        // Act
        await authorRepository.CreateAuthor(authorDto);
        await authorRepository.CreateAuthor(authorDto2);
        await authorRepository.Follow(authorDto.Name, authorDto2.Name);
        
        // Assert
        Assert.True(await authorRepository.IsFollowing(authorDto.Name, authorDto2.Name));
        Assert.False(await authorRepository.IsFollowing(authorDto2.Name, authorDto.Name));
    }
    
    [Fact]
    public async Task UnFollowing()
    {
        // Arrange
        var authorDto = new AuthorDTO { Name = "John Doe", Email = "email1" };
        var authorDto2 = new AuthorDTO { Name = "Not John Doe", Email = "gmail" };
        IAuthorRepository authorRepository = new Infrastructure.AuthorRepository(_dbContext);
        
        // Act
        await authorRepository.CreateAuthor(authorDto);
        await authorRepository.CreateAuthor(authorDto2);
        await authorRepository.Follow(authorDto.Name, authorDto2.Name);
        await authorRepository.Unfollow(authorDto.Name, authorDto2.Name);
        
        // Assert
        Assert.False(await authorRepository.IsFollowing(authorDto.Name, authorDto2.Name));
    }
}