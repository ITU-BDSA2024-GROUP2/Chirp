using Microsoft.VisualBasic.FileIO;

class Program
{
    public static void Main(string[] args)
    {
        string path = "data/chirp_cli_db.csv";
        
        if (args.Length > 0 && args[0].Equals("cheep"))
        {
            cheep(args, path);
        }
        
        parseCSV(path);
    }

    private static void parseCSV(string path)
    {
        //Source: https://dev.to/bristolsamo/c-csv-parser-step-by-step-tutorial-25ok
        
        TextFieldParser csvParser = new TextFieldParser(path);
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = true;
        csvParser.ReadLine(); // skips the first row with column names
        
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
        
        Console.WriteLine(author + " @ " + getCurrentTime(row) + ": " + message);
    }

    public static String getCurrentTime(string[] row)
    {
        var unixTimeStamp = int.Parse(row[2]);
        DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).DateTime.ToLocalTime(); // converts from unix to date time
        string formattedTime = dateTime.ToString("dd'/'MM'/'yy HH':'mm':'ss");
        return formattedTime;
    }
    

    public static void cheep(string[] args, string path)
    {
        StreamWriter streamWriter = File.AppendText(path);

        using (streamWriter)
        {
            streamWriter.WriteLine(Environment.UserName + ",\"" + args[1] + "\"," + DateTimeOffset.Now.ToUnixTimeSeconds());
        }
    }
}