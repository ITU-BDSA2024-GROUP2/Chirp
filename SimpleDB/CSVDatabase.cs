using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private string path = "data/chirp_cli_db.csv";
    
    public IEnumerable<T> Read(int? limit = null)
    {
        throw new NotImplementedException();
    }

    public void Store(T record)
    {
        using (var stream = File.Open(path, FileMode.Append))
        using (var writer = new StreamWriter(stream))
        using (var CSVWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            CSVWriter.WriteRecord(record); // Adds the Cheep to the CSV file
            writer.WriteLine(); // Creates new line after each append
        }
    }
}
