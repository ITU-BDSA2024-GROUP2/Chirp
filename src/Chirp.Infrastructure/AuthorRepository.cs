#nullable disable //fjern null warning
using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpDBContext _dbContext;
    
    public AuthorRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Author> CreateAuthor(AuthorDTO authorDto)
    {
        Author newAuthor = new() { UserName = authorDto.Name, Email = authorDto.Email };
        var queryResult = await _dbContext.Authors.AddAsync(newAuthor); // does not write to the database!

        await _dbContext.SaveChangesAsync(); // persist the changes in the database
        return queryResult.Entity;
    }
    
    public async Task<Author> FindAuthor(string authorName)
    {
        var query = from author in _dbContext.Authors
            where (author.UserName == authorName)
            select author;

        var result = await query.Distinct().FirstOrDefaultAsync();

        if (result == null)
        {
            throw new InvalidOperationException($"Author with name '{authorName}' was not found.");
        }

        return result;
    }

    public async Task<List<Author>> GetFollowingAuthors(string authorName)
    {
        var foundAuthor = await FindAuthor(authorName);
        var following = foundAuthor.Following.ToList();

        return following;
    }
}