using Chirp.Core;

namespace Chirp.Infrastructure;

public class CheepService : ICheepService
{

    private readonly ICheepRepository _cheepRepository;
    public CheepService(ICheepRepository cheepRepository)
    {
        _cheepRepository = cheepRepository;
    }
    
    public async Task<List<CheepDTO>> GetCheeps(int currentPage)
    {
        var cheeps = await _cheepRepository.GetCheeps(currentPage);
        return cheeps;
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string authorName, int currentPage)
    {
        var cheeps = await _cheepRepository.GetCheepsFromAuthor(authorName, currentPage);
        return cheeps;
    }
}