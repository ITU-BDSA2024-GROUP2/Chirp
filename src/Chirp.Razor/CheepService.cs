using Chirp.Razor;

public interface ICheepService 
{
    public Task<List<CheepDTO>> GetCheeps();
    public Task<List<CheepDTO>> GetCheepsFromAuthor(string authorName);
}

public class CheepService : ICheepService
{

    private readonly CheepRepository _cheepRepository;
    public CheepService(CheepRepository cheepRepository)
    {
        _cheepRepository = cheepRepository;
    }
    
    public async Task<List<CheepDTO>> GetCheeps()
    {
        var cheeps = await _cheepRepository.GetCheeps();
        return cheeps;
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string authorName)
    {
        var cheeps = await _cheepRepository.GetCheepsFromAuthor(authorName);
        return cheeps;
    }
}
