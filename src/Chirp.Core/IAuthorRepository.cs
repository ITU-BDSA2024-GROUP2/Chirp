namespace Chirp.Core;

public interface IAuthorRepository
{
    public Task<Author> CreateAuthor(AuthorDTO authorDto);
    public Task<Author> FindAuthor(AuthorDTO authorDto);
}