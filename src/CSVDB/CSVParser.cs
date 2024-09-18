using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace CSVDB;

public static class CSVParser
{

    public static List<T> Parse<T>(string path)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true, // The CSV file has a header row
        };
        
        // Source: https://joshclose.github.io/CsvHelper/getting-started/#reading-a-csv-file
        using (var streamReader = new StreamReader(path))
        using (var csvReader = new CsvReader(streamReader, config))
        {
            return csvReader.GetRecords<T>().ToList();
        }
    }
}