using DocoptNet;

using DocoptNet;
using SimpleDB;
 
namespace Chirp.CLI
{
    class Program
    {
        const string usage = @"Chirp CLI.
 
Usage:
  chirp read <limit>
  chirp cheep <message>
  chirp (-h | --help)
  chirp --version
 
Options:
  -h --help     Show this screen.
  --version     Show version.
";
        public static void Main(string[] args)
        {
            var arguments = new Docopt().Apply(usage, args, version: "1.0", exit: true)!;
 
            string test = arguments["read"]?.ToString();    // "read" command
            string test2 = arguments["<limit>"]?.ToString(); // <limit> argument

            Console.WriteLine(test);  // Prints "True" if "read" command is present
            Console.WriteLine(test2); // Prints the value of <limit>
            
            IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>();


            System.Collections.Generic.List<Cheep> cheeps = database.Read(20).ToList();
            UserInterface.PrintCheeps(cheeps);
        }
 
        private static void Cheep()
        {
 
        }
    }
}