using System.Globalization;
using Chirp.CLI;
using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private string path = "data/chirp_cli_db.csv";
    
    public IEnumerable<T> Read(int? limit = null)
    {
        if (limit.HasValue)
        {
            // Returns a new enumerable collection that contains the last count elements from source.
            // https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.takelast?view=net-8.0
            return CSVParser.Parse<T>(path).TakeLast(limit.Value).ToList();
        }

        return CSVParser.Parse<T>(path);
    }

    public void Store(T record)
    {
        using (var stream = File.Open(path, FileMode.Append))
        using (var writer = new StreamWriter(stream))
        using (var cswWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            cswWriter.WriteRecord(record); // Adds the Cheep to the CSV file
            writer.WriteLine(); // Creates new line after each append
        }
    }
}
