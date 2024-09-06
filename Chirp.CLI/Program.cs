using Chirp.CLI;
using Chirp.CLI.data;
using SimpleDB;

class Program
{
    
    public static void Main(string[] args)
    {
        string path = "data/chirp_cli_db.csv";
        
        IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>();
        Cheep cheep = new Cheep("nikolai", "test", 12345679);
        database.Store(cheep);

        List<Cheep> cheeps = CSVParser.Parse(path);
        UserInterface.PrintCheep(cheeps);
    }
}