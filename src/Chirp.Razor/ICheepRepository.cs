namespace Chirp.Razor;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetCheeps(int pageNumber, int pageSize);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int pageNumber, int pageSize);
}