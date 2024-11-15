namespace Chirp.Core;

public interface IAuthorRepository
{
    public Task<Author> CreateAuthor(AuthorDTO authorDto);
    public Task<Author> FindAuthor(string authorName);
    public Task Follow(string followingUserName, string followedUserName);
    
    public Task Unfollow(string unfollowingUserName, string unfollowedUserName);

    public Task<Boolean> IsFollowing(string userName, string authorName);

}