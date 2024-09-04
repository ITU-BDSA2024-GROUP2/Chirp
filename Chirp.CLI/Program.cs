using Chirp.CLI;
using Microsoft.VisualBasic.FileIO;

class Program
{
    
    public static void Main(string[] args)
    {
        string path = "data/chirp_cli_db.csv";
        
        if (args.Length > 0 && args[0].Equals("cheep"))
        {
            Cheep(args, path);
        }
        
        ParseCSV(path);
    }

    private static void ParseCSV(string path)
    {
        //Source: https://dev.to/bristolsamo/c-csv-parser-step-by-step-tutorial-25ok

        using (TextFieldParser csvParser = new TextFieldParser(path))
        {
            csvParser.SetDelimiters(",");
            csvParser.HasFieldsEnclosedInQuotes = true;
            csvParser.ReadLine(); // skips the first row with column names

            while (!csvParser.EndOfData)
            {
                string[] row = csvParser.ReadFields();
                UserInterface.PrintCheep(row);
            }
        }
    }

    public static void Cheep(string[] args, string path)
    {
        StreamWriter streamWriter = File.AppendText(path);

        using (streamWriter)
        {
            streamWriter.WriteLine(Environment.UserName + ",\"" + args[1] + "\"," + DateTimeOffset.Now.ToUnixTimeSeconds());
        }
    }
}