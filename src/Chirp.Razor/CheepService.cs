using Chirp.Razor;

public interface ICheepService
{
    public Task<List<CheepDTO>> GetCheeps(int pageNumber, int pageSize);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int pageNumber, int pageSize);
}

public class CheepService : ICheepService
{
    private readonly ICheepRepository _cheepRepository;  // Injecting the repository

    public CheepService(ICheepRepository cheepRepository)
    {
        _cheepRepository = cheepRepository;
    }
    
    public async Task<List<CheepDTO>> GetCheeps(int pageNumber, int pageSize)
    {
        List<CheepDTO> cheeps = await _cheepRepository.GetCheeps(pageNumber, pageSize);
        return cheeps;  
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int pageNumber, int pageSize)
    {
        List<CheepDTO> cheeps = await _cheepRepository.GetCheepsFromAuthor(author,pageNumber,pageSize);
        return cheeps;
    }

}
