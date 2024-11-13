namespace Chirp.Core;

public interface ICheepService 
{
    public Task<List<CheepDTO>> GetCheeps(int currentPage);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(string authorName, int currentPage);
    public Task<List<CheepDTO>> GetCheepsFromFollowers(string authorName, int currentPage, List<Author> follows);
    public Task CreateCheep(string authorName, string message);
}