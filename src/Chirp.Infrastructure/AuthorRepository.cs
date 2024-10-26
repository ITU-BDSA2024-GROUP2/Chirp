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
        Author newAuthor = new() { Name = authorDto.Name, Email = authorDto.Email };
        var queryResult = await _dbContext.Authors.AddAsync(newAuthor); // does not write to the database!

        await _dbContext.SaveChangesAsync(); // persist the changes in the database
        return queryResult.Entity;
    }
    
    public async Task<Author> FindAuthor(AuthorDTO authorDto)
    {
        var query = from author in _dbContext.Authors
            where (string.IsNullOrEmpty(authorDto.Name) || author.Name.ToLower().Contains(authorDto.Name.ToLower())) &&
                  (string.IsNullOrEmpty(authorDto.Email) || author.Email.ToLower().Contains(authorDto.Email.ToLower()))
            select author;

        var result = await query.Distinct().FirstOrDefaultAsync();

        return result;
    }
    
}