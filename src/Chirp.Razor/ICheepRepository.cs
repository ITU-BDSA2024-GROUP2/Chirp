namespace Chirp.Razor;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetCheeps();
    public Task<List<CheepDTO>> GetCheepsFromAuthor(string authorName);
}