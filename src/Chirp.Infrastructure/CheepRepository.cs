#nullable disable //fjern null warning
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
                Author = cheep.Author.UserName,
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
            where cheep.Author.UserName == authorName
            select new CheepDTO
            {
                Author = cheep.Author.UserName,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString("MM'/'dd'/'yy H':'mm':'ss")
            }).Skip(offset).Take(pageSize);
        var result = await query.ToListAsync();
        
        return result;
    }
    
    public async Task<List<CheepDTO>> GetCheepsFromFollowers(string userName, int currentPage)
    {
        int offset = (currentPage - 1) * pageSize;
        var user = await FindAuthorByName(userName);
        
        var query = (from cheep in _dbContext.Cheeps
            orderby cheep.TimeStamp descending
            where user.Following.Contains(cheep.Author) || cheep.Author.UserName == userName
            select new CheepDTO
            {
                Author = cheep.Author.UserName,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString("MM'/'dd'/'yy H':'mm':'ss")
            }).Skip(offset).Take(pageSize);
        var result = await query.ToListAsync();
        
        return result;
    }
    
    public async Task<Cheep>  CreateCheep(string authorName, string text)
    {
        Author author = await FindAuthorByName(authorName);
        
        Cheep newCheep = new()
        {
            Text = text,
            TimeStamp = DateTime.UtcNow, 
            Author = author,
        };
        
        var queryResult = await _dbContext.Cheeps.AddAsync(newCheep); // does not write to the database!

        await _dbContext.SaveChangesAsync(); // persist the changes in the database
        return newCheep;
    }

    public async Task<Author> FindAuthorByName(string name)
    {
        var query = from author in _dbContext.Authors
                .Include(a => a.Following)
                .Include(a => a.Followers)
            where (author.UserName == name)
            select author;

        var foundAuthor = await query.FirstOrDefaultAsync();

        if (foundAuthor == null)
        {
            throw new ValidationException($"Author {name} does not exist.");
        }

        return foundAuthor;
    }
}