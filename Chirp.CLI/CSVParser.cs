using System.Globalization;
using Chirp.CLI.data;
using CsvHelper;

namespace Chirp.CLI;

public static class CSVParser
{

    public static IEnumerable<Cheep> Parse(string path)
    {
        using (var streamReader = new StreamReader(path))
        using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
        {
            return csvReader.GetRecords<Cheep>();
        }
    }
    
}