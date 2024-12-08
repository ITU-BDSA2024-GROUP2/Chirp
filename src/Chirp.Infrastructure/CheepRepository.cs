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
    
    /// <summary>
    /// Gets all cheeps, and puts the newest at the top.
    /// </summary>
    /// <param name="currentPage"></param>
    /// <returns></returns>
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
    
    /// <summary>
    /// Method used for creating cheeps. The cheeps are checked for length
    /// and whitespace to see if the cheep is valid.
    /// If it is valid it creates a new cheep and inserts it into the database
    /// </summary>
    /// <param name="authorName"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
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
            TimeStamp = DateTime.Now, 
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

        var author = await FindAuthorByName(userName);
        var likeExists = false;
        if (author != null)
        {
            likeExists = await _dbContext.Likes
                .AnyAsync(l => l.CheepId == cheepId && l.AuthorId == author.Id);
        }
        
        if (!likeExists)
        {
            var like = new Like
            {
                AuthorId = author.Id,
                CheepId = cheepId
            };
           await  _dbContext.Likes.AddAsync(like);
        }
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task Unlike(string cheepId, string userName)
    {
        var author = await FindAuthorByName(userName);
        if (author != null)
        {
            var like = await _dbContext.Likes
                .FirstOrDefaultAsync(l => l.CheepId == cheepId && l.AuthorId == author.Id);
            _dbContext.Likes.Remove(like);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsLiked(string cheepId, string userName)
    {
        var author = await FindAuthorByName(userName);
        var like = await _dbContext.Likes.FirstOrDefaultAsync(l => l.CheepId == cheepId && l.AuthorId == author.Id);

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

    public async Task DeleteLikes(string userName)
    {
        var author = await FindAuthorByName(userName);
        var delete = from like in _dbContext.Likes
                where like.AuthorId == author.Id
                select like;

        if (delete.Any())
        {
            _dbContext.Likes.RemoveRange(delete);
        }

        await _dbContext.SaveChangesAsync();
    }
}