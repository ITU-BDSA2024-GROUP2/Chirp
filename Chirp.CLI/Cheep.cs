namespace Chirp.CLI.data;

public record Cheep(string Author, string Message, long Timestamp)
{

    public string GetFormattedTime(long timestamp)
    {
        DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime.ToLocalTime(); // converts from unix to date time
        string formattedTime = dateTime.ToString("dd'/'MM'/'yy HH':'mm':'ss");
        return formattedTime;
    }
    public override string ToString()
    {
        return $"{Author} @ {GetFormattedTime(Timestamp)}: {Message}";
    }
  
}