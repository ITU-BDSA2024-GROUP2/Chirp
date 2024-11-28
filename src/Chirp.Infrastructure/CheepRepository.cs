#nullable disable //fjern null warning
using System.ComponentModel.DataAnnotations;
using Chirp.Core;
using Microsoft.AspNetCore.Identity;
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
                Id = cheep.CheepId.ToString(),
                Author = cheep.Author.UserName,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString("MM'/'dd'/'yy H':'mm':'ss")
            }).Skip(offset).Take(pageSize);
        
        var result = await query.ToListAsync();
        return result;
    }
    
    public async Task<List<CheepDTO>> GetAllCheepsFromAuthor(string authorName)
    {
        var query = (from cheep in _dbContext.Cheeps
            orderby cheep.TimeStamp descending
            where cheep.Author.UserName == authorName
            select new CheepDTO
            {
                Id = cheep.CheepId.ToString(),
                Author = cheep.Author.UserName,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString("MM'/'dd'/'yy H':'mm':'ss")
            });
        
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
                Id = cheep.CheepId.ToString(),
                Author = cheep.Author.UserName,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString("MM'/'dd'/'yy H':'mm':'ss")
            }).Skip(offset).Take(pageSize);
        var result = await query.ToListAsync();
        
        return result;
    }
    
    public async Task<List<CheepDTO>> GetCheepsFromFollowersAndOwnCheeps(string userName, int currentPage)
    {
        int offset = (currentPage - 1) * pageSize;
        var user = await _dbContext.Authors
            .Include(a => a.Following)
            .FirstOrDefaultAsync(a => a.UserName == userName);
        
        if (user == null)
        {
            return new List<CheepDTO>();
        }
        
        var query = (from cheep in _dbContext.Cheeps
            orderby cheep.TimeStamp descending
            where user.Following.Contains(cheep.Author) || cheep.Author.UserName == userName
            select new CheepDTO
            {
                Id = cheep.CheepId.ToString(),
                Author = cheep.Author.UserName,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString("MM'/'dd'/'yy H':'mm':'ss")
            }).Skip(offset).Take(pageSize);
        var result = await query.ToListAsync();
        
        return result;
    }
    
    public async Task<Cheep>  CreateCheep(string authorName, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Cheep text cannot be empty.");
        }
        if (text.Length > 160)
        {
            throw new ArgumentException("Cheep text cannot exceed 160 characters.");
        }
        
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
    
    public async Task DeleteCheep(string cheepId, string userName)
    {
        var cheep = await _dbContext.Cheeps.FindAsync(cheepId);

        if (cheep.Author.UserName != userName)
        {
            throw new ArgumentException("Cheep does not belong to this user.");
        }
        if (cheep != null)
        { 
            _dbContext.Cheeps.Remove(cheep);
        }
        
        await _dbContext.SaveChangesAsync();
    }

    private async Task<Cheep> GetCheep(string cheepId)
    {
        var query = from cheep in _dbContext.Cheeps
                .Include(c => c.Likes)
            where (cheep.CheepId.ToString() == cheepId)
            select cheep;

        var result = await query.Distinct().FirstOrDefaultAsync();
        return result;
    }

    public async Task Like(string cheepId, string userName)
    {
        var foundCheep = await GetCheep(cheepId);
        
        if (foundCheep == null)
        {
            throw new ArgumentException("Cheep not found");
        }
        
        if (foundCheep.Likes.Any(l => l.Author == userName))
        {
            throw new InvalidOperationException("User has already liked this Cheep");
        }
        
        foundCheep.Likes.Add(new Like
        {
            Author = userName,
            CheepId = cheepId,
        });

        await _dbContext.SaveChangesAsync();
    }
    
    public async Task Unlike(string cheepId, string userName)
    {
        var foundCheep = await GetCheep(cheepId);
        
        if (foundCheep == null)
        {
            throw new ArgumentException("Cheep not found");
        }
        
        if (!foundCheep.Likes.Any(l => l.Author == userName))
        {
            throw new InvalidOperationException("User has not liked this Cheep");
        }
        
        var like = foundCheep.Likes.FirstOrDefault(l => l.Author == userName);
        if (like == null)
        {
            throw new InvalidOperationException("Like not found");
        }
        foundCheep.Likes.Remove(like);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsLiked(string cheepId, string userName)
    {
        var foundCheep = await GetCheep(cheepId);
        Console.WriteLine(cheepId);
        
        return foundCheep != null && foundCheep.Likes.Any(l => l.Author == userName);
    }

    public async Task<Author> FindAuthorByName(string name)
    {
        var query = from author in _dbContext.Authors
            where (author.UserName == name)
            select author;

        var foundAuthor = await query.FirstOrDefaultAsync();

        return foundAuthor;
    }
}