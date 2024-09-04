namespace Chirp.CLI;

public static class UserInterface
{
    public static void PrintCheep(string[] row)
    {
        var author = row[0];
        var message = row[1];
        
        Console.WriteLine(author + " @ " + GetCurrentTime(row) + ": " + message);
    }

    private static String GetCurrentTime(string[] row)
    {
        var unixTimeStamp = int.Parse(row[2]);
        DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).DateTime.ToLocalTime(); // converts from unix to date time
        string formattedTime = dateTime.ToString("dd'/'MM'/'yy HH':'mm':'ss");
        return formattedTime;
    }
}