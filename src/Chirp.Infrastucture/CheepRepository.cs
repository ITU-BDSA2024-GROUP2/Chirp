using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;
    private const int pageSize = 32;
    
    public CheepRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<CheepDTO>> GetCheeps(int currentPage)
    {
        int offset = (currentPage - 1) * pageSize;

        var query = (from cheep in _dbContext.Cheeps
            orderby cheep.TimeStamp descending
            select new CheepDTO
            {
                Author = cheep.Author!.Name,
                Text = cheep.Text!,
                TimeStamp = cheep.TimeStamp.ToString("MM'/'dd'/'yy H':'mm':'ss")
            }).Skip(offset).Take(pageSize);
        
        var result = await query.ToListAsync();
        return result;
    }
    
    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string authorName, int currentPage)
    {
        int offset = (currentPage - 1) * pageSize;

        var query = (from cheep in _dbContext.Cheeps
            orderby cheep.TimeStamp descending
            where cheep.Author!.Name == authorName
            select new CheepDTO
            {
                Author = cheep.Author!.Name,
                Text = cheep.Text!,
                TimeStamp = cheep.TimeStamp.ToString("MM'/'dd'/'yy H':'mm':'ss")
            }).Skip(offset).Take(pageSize);
        var result = await query.ToListAsync();
        
        return result;
    }
    
    public async Task<Author> CreateAuthor(AuthorDTO author)
    {
        // Check if author already exists
        var existingAuthor = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Name == author.Name || a.Email == author.Email);
        
        if (existingAuthor != null)
        {
            Author newAuthor = new() { Name = author.Name, Email = author.Email };
            var queryResult = await _dbContext.Authors.AddAsync(newAuthor); // does not write to the database!

            await _dbContext.SaveChangesAsync(); // persist the changes in the database
            return queryResult.Entity;
        }

        return existingAuthor!;
    }
    
    public async Task<Cheep> CreateCheep(CheepDTO cheepDto, AuthorDTO authorDto) // Måske fjern authorDTO
    {
        var author = await FindAuthor(authorDto.Name, authorDto.Email);

        if (author == null)
        {
            author = await CreateAuthor(new AuthorDTO { Name = authorDto.Name, Email = authorDto.Email });
        }
        
        Cheep newCheep = new() { Text = cheepDto.Text, TimeStamp = DateTime.UtcNow, Author = author, AuthorId = author.AuthorId };
        var queryResult = await _dbContext.Cheeps.AddAsync(newCheep); // does not write to the database!

        await _dbContext.SaveChangesAsync(); // persist the changes in the database
        return queryResult.Entity;
    }
    
    public async Task<Author> FindAuthor(string authorName, string authorEmail)
    {
        var query = from author in _dbContext.Authors
            where (string.IsNullOrEmpty(authorName) || author.Name!.ToLower().Contains(authorName.ToLower())) &&
                  (string.IsNullOrEmpty(authorEmail) || author.Email!.ToLower().Contains(authorEmail.ToLower()))
                  select author;

        var result = await query.Distinct().FirstOrDefaultAsync();

        return result;
    }
}