using Chirp.CLI.data;

namespace Chirp.CLI;

public static class UserInterface
{
    public static void PrintCheep(List<Cheep> cheeps)
    {
        foreach (var cheep in cheeps)
        {
            Console.WriteLine(cheep.toString());
        }
    }
}