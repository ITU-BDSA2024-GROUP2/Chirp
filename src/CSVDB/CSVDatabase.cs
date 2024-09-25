using System.Globalization;
using CsvHelper;

namespace CSVDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private static readonly CSVDatabase<T> instance = new CSVDatabase<T>();
    private string _filePath;

    private CSVDatabase()
    {
        _filePath = "../../../../../data/chirp_cli_db.csv";
    }

    public static CSVDatabase<T> Instance
    {
        get
        {
            return instance;
        }
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        if (limit.HasValue)
        {
            // Returns a new enumerable collection that contains the last count elements from source.
            // https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.takelast?view=net-8.0
            return CSVParser.Parse<T>(_filePath).TakeLast(limit.Value);
        }

        return CSVParser.Parse<T>(_filePath);
    }

    public void Store(T record)
    {
        using (var stream = File.Open(_filePath, FileMode.Append))
        using (var writer = new StreamWriter(stream))
        using (var cswWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            cswWriter.WriteRecord(record); // Adds the Cheep to the CSV file
            writer.WriteLine(); // Creates new line after each append
        }
    }
    
    public void SetFilePath(string filePath)
    {
        _filePath = filePath;
    }
}
