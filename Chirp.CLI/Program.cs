using SimpleDB;
using DocoptNet;

const string usage = @"Chirp CLI version.

Usage:
  chirp read <limit>
  chirp cheep <message>
  chirp (-h | --help)
  chirp --version

Options:
  -h --help     Show this screen.
  --version     Show version.
";

var arguments = new Docopt().Apply(usage, args, version: "1.0", exit: true)!;

namespace Chirp.CLI
{
    class Program
    {
    
        public static void Main(string[] args)
        {
            IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>();
            List<Cheep> cheeps = database.Read(20).ToList();
            UserInterface.PrintCheeps(cheeps);
        }
    }
}