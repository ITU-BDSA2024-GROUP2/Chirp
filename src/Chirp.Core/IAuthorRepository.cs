namespace Chirp.Core;

public interface IAuthorRepository
{
    public Task<Author> CreateAuthor(AuthorDTO authorDto);
    public Task<Author> FindAuthor(string authorName);
    
    public Task<List<Author>> GetFollowingAuthors(string authorName);
}