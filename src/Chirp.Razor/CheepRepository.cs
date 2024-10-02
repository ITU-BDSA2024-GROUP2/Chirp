using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;
    
    public CheepRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<Cheep>> GetCheeps()
    {
        var query = from cheep in _dbContext.Cheeps 
            select cheep;
        
        var result = await query.ToListAsync();
        return result;
    }

    public async Task<List<Cheep>> GetCheepsFromAuthor(int authorId)
    {
        throw new NotImplementedException();
    }
}