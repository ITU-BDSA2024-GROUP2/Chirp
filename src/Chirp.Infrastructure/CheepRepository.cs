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
    
    public async Task<List<CheepDTO>> GetCheepsByNewest(int currentPage)
    {
        int offset = (currentPage - 1) * pageSize;

        var query = (from cheep in _dbContext.Cheeps
            orderby cheep.TimeStamp descending
            select new CheepDTO
            {
                Id = cheep.CheepId,
                Author = cheep.Author.UserName,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString("MM'/'dd'/'yy H':'mm':'ss"),
                LikeCount = cheep.Likes.Count.ToString(),
            }).Skip(offset).Take(pageSize);
        
        var result = await query.ToListAsync();
        return result;
    }
    
    public async Task<List<CheepDTO>> GetCheeps(int currentPage)
    {
        int offset = (currentPage - 1) * pageSize;

        var query = (from cheep in _dbContext.Cheeps
            orderby cheep.Likes.Count descending
            select new CheepDTO
            {
                Id = cheep.CheepId,
                Author = cheep.Author.UserName,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString("MM'/'dd'/'yy H':'mm':'ss"),
                LikeCount = cheep.Likes.Count.ToString(),
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
                Id = cheep.CheepId,
                Author = cheep.Author.UserName,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString("MM'/'dd'/'yy H':'mm':'ss"),
                LikeCount = cheep.Likes.Count.ToString(),
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
                Id = cheep.CheepId,
                Author = cheep.Author.UserName,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString("MM'/'dd'/'yy H':'mm':'ss"),
                LikeCount = cheep.Likes.Count.ToString(),
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
                Id = cheep.CheepId,
                Author = cheep.Author.UserName,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString("MM'/'dd'/'yy H':'mm':'ss"),
                LikeCount = cheep.Likes.Count.ToString(),
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
        
        await _dbContext.Cheeps.AddAsync(newCheep); // does not write to the database!

        await _dbContext.SaveChangesAsync(); // persist the changes in the database
        return newCheep;
    }
    
    public async Task DeleteCheep(string cheepId, string userName)
    {
        var cheep = await _dbContext.Cheeps
            .Include(c => c.Author)
            .FirstOrDefaultAsync(c => c.CheepId == cheepId);
        
        if (cheep != null && cheep.Author.UserName == userName)
        { 
            _dbContext.Cheeps.Remove(cheep);
        }
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task Like(string cheepId, string userName)
    {
        var cheep = await _dbContext.Cheeps
            .Where(c => c.CheepId == cheepId)
            .Select(c => c.Author)
            .FirstOrDefaultAsync();
        
        if (cheep == null || cheep.UserName == userName)
        {
            return;
        }
        
        var likeExists = await _dbContext.Likes
            .AnyAsync(l => l.CheepId == cheepId && l.Author == userName);

        if (!likeExists)
        {
            var like = new Like
            {
                Author = userName,
                CheepId = cheepId
            };
           await  _dbContext.Likes.AddAsync(like);
        }
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task Unlike(string cheepId, string userName)
    {
        var like = await _dbContext.Likes
            .FirstOrDefaultAsync(l => l.CheepId == cheepId && l.Author == userName);

        if (like == null)
        {
            return;
        }

        _dbContext.Likes.Remove(like);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsLiked(string cheepId, string userName)
    {
        var like = await _dbContext.Likes.FirstOrDefaultAsync(l => l.CheepId == cheepId && l.Author == userName);

        return like != null;
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