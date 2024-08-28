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
        //Source: https://dev.to/bristolsamo/c-csv-parser-step-by-step-tutorial-25ok
        
        TextFieldParser csvParser = new TextFieldParser(path);
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = true;
        csvParser.ReadLine(); // skips the first row with column names

        int i = 0;
        while (!csvParser.EndOfData)
        {
            string[] row = csvParser.ReadFields();
            displayCheep(row);

        }
    }

    private static void displayCheep(string[] row)
    {
        var author = row[0];
        var message = row[1];
        
        var unixTimeStamp = int.Parse(row[2]);
        DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).DateTime;
        string formattedTime = dateTime.ToString("dd'/'MM'/'yy HH':'mm':'ss");
        
        Console.WriteLine(author + " @ " + formattedTime + ": " + message);
    }
}