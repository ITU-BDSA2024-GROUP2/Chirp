namespace Chirp.CLI;

public static class UserInterface
{
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (var cheep in cheeps)
        {
            Console.WriteLine(cheep.ToString());
        }
    }
}