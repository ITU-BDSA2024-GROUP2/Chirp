namespace Chirp.Core;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetCheeps(int currentPage);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(string authorName, int currentPage);
    public Task<List<CheepDTO>> GetCheepsFromFollowers(string userName, int currentPage);
    public Task<Cheep> CreateCheep(string authorName, string message);
    public Task<Author> FindAuthorByName(string name);
    public Task DeleteCheep(string cheepId);
    public Task<List<CheepDTO>> GetAllCheepsFromAuthor(string authorName);
}