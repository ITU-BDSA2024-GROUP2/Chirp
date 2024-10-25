namespace Chirp.Core;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetCheeps(int currentPage);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(string authorName, int currentPage);
    public Task<Cheep> CreateCheep(CheepDTO cheepDto);
    public Task<Author> FindAuthorByName(string name);
}