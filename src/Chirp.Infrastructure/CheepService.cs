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

    public async Task<List<CheepDTO>> GetCheepsFromFollowers(string authorName, int currentPage, List<Author> follows)
    {
        var cheeps = await _cheepRepository.GetCheepsFromFollowers(authorName, currentPage, follows);
        return cheeps;
    }

    public async Task CreateCheep(string authorName, string message)
    {
        await _cheepRepository.CreateCheep(authorName, message);
    }
}