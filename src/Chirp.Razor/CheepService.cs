using Chirp.Razor;

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    public List<CheepViewModel> GetCheeps()
    {
        DBFacade dbFacade = new DBFacade();
        List<CheepViewModel> cheeps = dbFacade.ReadCheeps();
        
        return cheeps;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        DBFacade dbFacade = new DBFacade();
        List<CheepViewModel> cheeps = dbFacade.ReadCheeps();
        
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
