namespace Chirp.Razor;

public record CheepViewModel(string Author, string Message, string Timestamp)
{
    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
    
    public override string ToString()
    {
        return $"{Author} @ {Timestamp}: {Message}";
    }

    public static CheepViewModel CreateCheep(string author, string message, double timestamp)
    {
        return new CheepViewModel(author, message, UnixTimeStampToDateTimeString(timestamp));
    }
  
}