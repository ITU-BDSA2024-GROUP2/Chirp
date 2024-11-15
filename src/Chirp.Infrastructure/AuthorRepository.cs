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

    public async Task Follow(string followingUserName, string followedUserName)
    {
        var followingAuthor = await FindAuthor(followingUserName);
        var followedAuthor = await FindAuthor(followedUserName);
        
        if (followingAuthor == null || followedAuthor == null)
        {
            throw new ArgumentException("One or both authors do not exist.");
        }
        
        if (!followingAuthor.Following.Contains(followedAuthor))
        {
            followingAuthor.Following.Add(followedAuthor);
        }

        if (!followedAuthor.Followers.Contains(followingAuthor))
        {
            followedAuthor.Followers.Add(followingAuthor);
        }
        
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task Unfollow(string unfollowingUserName, string unfollowedUserName)
    {
        var unfollowingAuthor = await FindAuthor(unfollowingUserName);
        var unfollowedAuthor = await FindAuthor(unfollowedUserName);
        
        if (unfollowingAuthor == null || unfollowedAuthor == null)
        {
            throw new ArgumentException("One or both authors do not exist.");
        }
        
        if (unfollowingAuthor.Following.Contains(unfollowedAuthor))
        {
            unfollowingAuthor.Following.Remove(unfollowedAuthor);
        }

        if (unfollowedAuthor.Followers.Contains(unfollowingAuthor))
        {
            unfollowedAuthor.Followers.Remove(unfollowingAuthor);
        }
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Author>> GetFollowing(string authorName)
    {
        var foundAuthor = await FindAuthor(authorName);
        var following = foundAuthor.Following.ToList();

        return following;
    }

    public async Task<List<Author>> GetFollowers(string authorName)
    {
        var foundAuthor = await FindAuthor(authorName);
        var followers = foundAuthor.Followers.ToList();
        
        return followers;
    }

    public async Task<bool> IsFollowing(string authorName, string otherAuthor)
    {
        var foundOtherAuthor = await FindAuthor(otherAuthor);
        var following = await GetFollowing(authorName);
        
        return following.Contains(foundOtherAuthor);
    }
}