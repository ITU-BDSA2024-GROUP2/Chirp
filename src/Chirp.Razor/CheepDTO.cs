namespace Chirp.Razor;

public class CheepDTO()
{
    public string Author { get; set; }
    public string Text { get; set; }
    public string TimeStamp { get; set; }
    
    private static string UnixTimeStampToDateTimeString(DateTime unixTimeStamp)
    {
        string formattedDate = unixTimeStamp.ToString("MM'/'dd'/'yy H':'mm':'ss");
        return formattedDate;
    }
    
    public static CheepDTO CreateCheepDTO(string author, string text, DateTime timestamp)
    {
        CheepDTO cheepDto = new CheepDTO()
        {
            Author = author,
            Text = text,
            TimeStamp = UnixTimeStampToDateTimeString(timestamp)
        };
        
        return cheepDto;
    }
}