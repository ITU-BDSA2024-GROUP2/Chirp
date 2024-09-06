using Chirp.CLI;
using Chirp.CLI.data;
using SimpleDB;

class Program
{
    
    public static void Main(string[] args)
    {
        IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>();
        Cheep cheep = new Cheep("nikolai", "New", 12345679);
        database.Store(cheep);

        //List<Cheep> cheeps = CSVParser.Parse(path);
        List<Cheep> cheeps = database.Read(20).ToList();
        UserInterface.PrintCheep(cheeps);
    }
}