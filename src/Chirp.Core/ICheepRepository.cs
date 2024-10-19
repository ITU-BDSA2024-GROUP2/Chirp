namespace Chirp.Core;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetCheeps(int currentPage);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(string authorName, int currentPage);
    public Task<Author> CreateAuthor(AuthorDTO authorDto);
    public Task<Cheep> CreateCheep(CheepDTO cheepDto, AuthorDTO authorDto);
    public Task<Author> FindAuthor(AuthorDTO authorDto);
}