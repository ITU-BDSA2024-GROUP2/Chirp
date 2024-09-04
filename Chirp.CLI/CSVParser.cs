using System.Globalization;
using Chirp.CLI.data;
using CsvHelper;

namespace Chirp.CLI;

public static class CSVParser
{

    public static List<Cheep> Parse(string path)
    {
        // Source: https://joshclose.github.io/CsvHelper/getting-started/#reading-a-csv-file
        using (var streamReader = new StreamReader(path))
        using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
        {
            return csvReader.GetRecords<Cheep>().ToList();
        }
    }
    
}