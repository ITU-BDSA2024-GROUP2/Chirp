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
                .Include(a => a.Following)
                .Include(a => a.Followers)
                .Include(a => a.ProfilePicture)
            where (author.UserName == authorName)
            select author;

        var result = await query.Distinct().FirstOrDefaultAsync();


        if (result == null)
        {
            throw new InvalidOperationException($"Author with name '{authorName}' was not found.");
        }

        return result;
    }

    public async Task Follow(string userName, string authorName)
    {
        if (userName == authorName)
        {
            return;
        }
        
        var user = await FindAuthor(userName);
        var author = await FindAuthor(authorName);
        
        if (!user.Following.Contains(author))
        {
            user.Following.Add(author);
        }
        
        if (!author.Followers.Contains(user))
        {
            author.Followers.Add(user);
        }
        
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task Unfollow(string userName, string authorName)
    {
        if (userName == authorName)
        {
            return;
        }
        
        var user = await FindAuthor(userName);
        var author = await FindAuthor(authorName);
        
        if (user.Following.Contains(author))
        {
            user.Following.Remove(author);
        }

        if (author.Followers.Contains(user))
        {
            author.Followers.Remove(user);
        }
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsFollowing(string userName, string authorName)
    {
        if (userName == authorName)
        {
            return false;
        }
        
        var author = await FindAuthor(authorName);
        var user = await FindAuthor(userName);
        
        return user.Following.Contains(author);
    }

    public async Task<List<string>> GetFollowing(string userName)
    {
        var query = from author in _dbContext.Authors
            where author.UserName == userName
            from following in author.Following
            select following.UserName;
        
        var result = await query.ToListAsync();
        return result;
    }
    
    public async Task<List<string>> GetFollowers(string userName)
    {
        var query = from author in _dbContext.Authors
            where author.UserName == userName
            from follower in author.Followers
            select follower.UserName;
        
        var result = await query.ToListAsync();
        return result;
    }
    
    public async Task<int> GetFollowerCount(string userName)
    {
        var query = from author in _dbContext.Authors
            where author.UserName == userName
            from follower in author.Followers
            select follower.UserName;
        
        var result = await query.ToListAsync();
        return result.Count;
    }
}