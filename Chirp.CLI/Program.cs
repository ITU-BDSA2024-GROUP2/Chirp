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

        private static IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>();

        public static void Main(string[] args)
        {
            var arguments = new Docopt().Apply(usage, args, version: "Chirp 1.0", exit: true)!;

            if (arguments["read"].IsTrue)
            {
                if (!int.TryParse(arguments["<limit>"].ToString(), out int limit) || limit < 1)
                {
                    Console.WriteLine("Bad input: <limit> must be an integer greater than 0.");
                    return;
                }
                RunReadCommand(limit);
            }
            else if (arguments["cheep"].IsTrue)
            {
                string message = arguments["<message>"].ToString();
                RunCheepCommand(message);
            }
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