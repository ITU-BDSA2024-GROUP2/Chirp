using Chirp.Razor;

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    private readonly DBFacade _dbFacade = new DBFacade();
    public List<CheepViewModel> GetCheeps()
    {
        List<CheepViewModel> cheeps = _dbFacade.ReadCheeps();
        
        return cheeps;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        List<CheepViewModel> cheeps = _dbFacade.ReadCheepsFromAuthor(author);
        
        return cheeps;
    }

}
