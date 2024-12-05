namespace Chirp.Core;

public interface IAuthorRepository
{
    public Task<Author> CreateAuthor(AuthorDTO authorDto);
    public Task<Author> FindAuthor(string authorName);
    public Task Follow(string userName, string authorName);
    
    public Task Unfollow(string userName, string authorName);

    public Task<Boolean> IsFollowing(string userName, string authorName);
    public Task<List<string>> GetFollowing(string userName);
    public Task<List<string>> GetFollowers(string userName);
    public Task<int> GetFollowerCount(string userName);
    public Task ChangeProfilePicture(string authorName, string? profilePictureLink);
    public Task<string?> GetProfilePicture(string? authorName);
}