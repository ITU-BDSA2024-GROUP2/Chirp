using Chirp.Razor;

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps(int pageNumber, int pageSize);
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int pageNumber, int pageSize);
}

public class CheepService : ICheepService
{
    public List<CheepViewModel> GetCheeps(int pageNumber, int pageSize)
    {
        DBFacade dbFacade = new DBFacade();
        List<CheepViewModel> cheeps = dbFacade.ReadCheeps(pageNumber, pageSize);
        
        return cheeps;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int pageNumber, int pageSize)
    {
        DBFacade dbFacade = new DBFacade();
        List<CheepViewModel> cheeps = dbFacade.ReadCheeps(pageNumber, pageSize);
        
        // filter by the provided author name
        return cheeps.Where(x => x.Author == author).ToList();
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
