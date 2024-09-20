using DocoptNet;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

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

        private const string BaseUrl = "http://localhost:5282";

        private static readonly HttpClient Client = new()
        {
            BaseAddress = new Uri(BaseUrl),
        };
        
        static int ShowHelp(string help) { Console.WriteLine(help); return 0; }
        static int ShowVersion(string version) { Console.WriteLine(version); return 0; }
        static int OnError(string usage) { Console.Error.WriteLine(usage); return 1; }

        public static void Main(string[] args)
        {
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            //Source: https://docopt.github.io/docopt.net/dev/#api
            
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
                RunReadCommand(limit).Wait();
            }
            else if (arguments["cheep"].IsTrue)
            {
                string message = arguments["<message>"].ToString();
                RunCheepCommand(message).Wait();
            }
            return 0;
        }

        private static async Task RunReadCommand(int limit)
        {
            var cheeps = await Client.GetFromJsonAsync<List<Cheep>>("/cheeps");
            
            if (cheeps != null) UserInterface.PrintCheeps(cheeps);
        }

        private static async Task RunCheepCommand(string message)
        {
            var newCheep = Cheep.CreateCheep(message);
            
            using var response = await Client.PostAsJsonAsync($"/cheep", newCheep);
            response.EnsureSuccessStatusCode();
            
            Console.WriteLine($"Post successful: {newCheep.ToString()}");
        }
    }
}