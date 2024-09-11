using DocoptNet;
using CSVDB;

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

        private static IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>();
        
        static int ShowHelp(string help) { Console.WriteLine(help); return 0; }
        static int ShowVersion(string version) { Console.WriteLine(version); return 0; }
        static int OnError(string usage) { Console.Error.WriteLine(usage); return 1; }

        public static void Main(string[] args)
        {
            // https://docopt.github.io/docopt.net/dev/#api
            
            Docopt.CreateParser(usage)
                .WithVersion("Chirp 1.0")
                .Parse(args)
                .Match(Run,
                    result => ShowHelp(result.Help),
                    result => ShowVersion(result.Version),
                    result => OnError(result.Usage));
        }

        public static int Run(IDictionary<string, ArgValue> arguments)
        {
            if (arguments["read"].IsTrue)
            {
                if (!int.TryParse(arguments["<limit>"].ToString(), out int limit) || limit < 1)
                {
                    Console.WriteLine("Bad input: <limit> must be an integer greater than 0.");
                    return 1;
                }
                RunReadCommand(limit);
            }
            else if (arguments["cheep"].IsTrue)
            {
                string message = arguments["<message>"].ToString();
                RunCheepCommand(message);
            }
            return 0;
        }

        private static void RunReadCommand(int limit)
        {
            List<Cheep> cheeps = database.Read(limit).ToList();
            UserInterface.PrintCheeps(cheeps);
        }

        private static void RunCheepCommand(string message)
        {
            database.Store(Cheep.CreateCheep(message));
        }
    }
}