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
            IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>();
            var arguments = new Docopt().Apply(usage, args, version: "1.0", exit: true)!;
            
            try
            {
                if (arguments["read"] != null)
                {
                    List<Cheep> cheeps = database.Read(int.Parse(arguments["<limit>"].ToString())).ToList();
                    UserInterface.PrintCheeps(cheeps);
                }
                else if (arguments["cheep"] != null)
                {
                    string message = arguments["<message>"].ToString();
                    database.Store(CreateCheep(message));
                } else if (arguments["-h"] != null || arguments["--help"] != null)
                {
                    Console.WriteLine(usage);
                } else if (arguments["--version"] != null)
                {
                    Console.WriteLine("Chirp CLI 1.0");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid arguments");
                Console.WriteLine(usage);
            }
        }

        private static Cheep CreateCheep(string message)
        {
            return new Cheep(Environment.UserName, message, DateTimeOffset.Now.ToUnixTimeMilliseconds());
        }
    }
}