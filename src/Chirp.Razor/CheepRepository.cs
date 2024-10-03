using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;
    
    public CheepRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<CheepDTO>> GetCheeps(int pageNumber, int pageSize)
    {   
        int offset = (pageNumber - 1) * pageSize;
        
        var query = from cheep in _dbContext.Cheeps
            select new CheepDTO
            {
                Author = cheep.Author.Name,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString()
            };
        
        query = query.Skip(offset).Take(pageSize);
        
        try
        {
            var result = await query.ToListAsync();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new List<CheepDTO>();    
        }
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int pageNumber, int pageSize)
    {
        int offset = (pageNumber - 1) * pageSize;
        int authorId = await ConvertAuthorToId(author);
        
        var query = from cheep in _dbContext.Cheeps
            where cheep.Author.AuthorId == authorId
            orderby cheep.TimeStamp descending
            select new CheepDTO
            {
                Author = cheep.Author.Name,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString()
            };
        
        query = query.Skip(offset).Take(pageSize);
        
        try
        {
            var result = await query.ToListAsync();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new List<CheepDTO>();    
        }
    }

    //På nuværende tidspunkt bruger frontenddelen kun navne, hvorfor denne funktion er nødvendig. i linket f.eks.
    //Antager det er at foretrække at linket måske skal anvende id istedet, hvor denne funktion så kan fjernes.
    public async Task<int> ConvertAuthorToId(string author) 
    {
        var list = await (from a in _dbContext.Authors where a.Name == author select a).ToListAsync(); //lav liste af authors med dette navn

        if (list.Count == 0)
        {
            throw new Exception($"Author {author} is not found");
        }

        if (list.Count == 1)
        {
           return list.First().AuthorId; 
        }
        
        throw new Exception($"Flere brugere med dette navn {author}");
        
    }
}