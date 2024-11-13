namespace Chirp.Core;

public interface IAuthorService
{
    public Task<List<Author>> GetFollowingAuthors(string authorName);
}