using Microsoft.VisualBasic.FileIO;

class Program
{
    public static void Main(string[] args)
    {
        var path = "/Users/nikolai/Chirp/Chirp.CLI/data/chirp_cli_db.csv";
        parseCSV(path);
    }

    private static void parseCSV(string path)
    {
        TextFieldParser csvParser = new TextFieldParser(path);
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = true;

        while (!csvParser.EndOfData)
        {
            string[] rows = csvParser.ReadFields();
            displayCheep(rows);
            
        }
    }

    private static void displayCheep(string[] rows)
    {
        foreach (String cheep in rows)
        {
            Console.WriteLine(cheep);
        }
    }
}