using SimpleDB;

namespace Chirp.CLI;

class Program
{
    
    public static void Main(string[] args)
    {
        IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>();
        List<Cheep> cheeps = database.Read(20).ToList();
        UserInterface.PrintCheeps(cheeps);
    }
}