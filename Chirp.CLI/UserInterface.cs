using Chirp.CLI.data;

namespace Chirp.CLI;

public static class UserInterface
{
    public static void PrintCheep(List<Cheep> cheeps)
    {
        foreach (var cheep in cheeps)
        {
            Console.WriteLine(cheep.Author + " @ " + unixToLocalTime(cheep.Timestamp) + ": " + cheep.Message);
        }
    }

    private static String unixToLocalTime(long timestamp)
    {
        DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime.ToLocalTime(); // converts from unix to date time
        string formattedTime = dateTime.ToString("dd'/'MM'/'yy HH':'mm':'ss");
        return formattedTime;
    }
}