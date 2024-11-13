namespace Chirp.Core;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetCheeps(int currentPage);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(string authorName, int currentPage);
    public Task<List<CheepDTO>> GetCheepsFromFollowers(string authorName, int currentPage, List<Author> follows);
    public Task<Cheep> CreateCheep(string authorName, string message);
    public Task<Author> FindAuthorByName(string name);
}