using System.ComponentModel.DataAnnotations;
using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;
    private const int pageSize = 32;
    
    public CheepRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<CheepDTO>> GetCheeps(int currentPage)
    {
        int offset = (currentPage - 1) * pageSize;

        var query = (from cheep in _dbContext.Cheeps
            orderby cheep.TimeStamp descending
            select new CheepDTO
            {
                Author = cheep.Author.Name,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString("MM'/'dd'/'yy H':'mm':'ss")
            }).Skip(offset).Take(pageSize);
        
        var result = await query.ToListAsync();
        return result;
    }
    
    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string authorName, int currentPage)
    {
        int offset = (currentPage - 1) * pageSize;

        var query = (from cheep in _dbContext.Cheeps
            orderby cheep.TimeStamp descending
            where cheep.Author.Name == authorName
            select new CheepDTO
            {
                Author = cheep.Author.Name,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString("MM'/'dd'/'yy H':'mm':'ss")
            }).Skip(offset).Take(pageSize);
        var result = await query.ToListAsync();
        
        return result;
    }
    
    public async Task<Author> CreateAuthor(AuthorDTO authorDto)
    {
        // Check if author already exists
        var existingAuthor = await FindAuthor(authorDto);
        
        if (existingAuthor == null)
        {
            Author newAuthor = new() { Name = authorDto.Name, Email = authorDto.Email };
            var queryResult = await _dbContext.Authors.AddAsync(newAuthor); // does not write to the database!

            await _dbContext.SaveChangesAsync(); // persist the changes in the database
            return queryResult.Entity;
        }

        return existingAuthor!;
    }
    
    public async Task<Cheep> CreateCheep(CheepDTO cheepDto, AuthorDTO authorDto) // Måske fjern authorDTO
    {
        var author = await CreateAuthor(authorDto); // returns author if already existing
        
        Cheep newCheep = new()
        {
            Text = cheepDto.Text, 
            TimeStamp = DateTime.UtcNow, 
            Author = author, 
            AuthorId = author.AuthorId
        };
        
        var validationResults = newCheep.Validate();
        if (validationResults.Any())
        {
            throw new ValidationException("Cheep is invalid: " +
                                          string.Join(", ", validationResults.Select(v => v.ErrorMessage)));
        }
        
        var queryResult = await _dbContext.Cheeps.AddAsync(newCheep); // does not write to the database!

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