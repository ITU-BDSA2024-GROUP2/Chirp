using Chirp.Razor;

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps(int pageNumber, int pageSize);
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int pageNumber, int pageSize);
}

public class CheepService : ICheepService
{
    private readonly DBFacade _dbFacade;
    public CheepService(DBFacade dbFacade)
    {
        _dbFacade = dbFacade;
    }
    public List<CheepViewModel> GetCheeps(int pageNumber, int pageSize)
    {
        List<CheepViewModel> cheeps = _dbFacade.ReadCheeps(pageNumber, pageSize);
        return cheeps;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int pageNumber, int pageSize)
    {
        List<CheepViewModel> cheeps = _dbFacade.ReadCheepsFromAuthor(author, pageNumber, pageSize);
        return cheeps;
    }

}
