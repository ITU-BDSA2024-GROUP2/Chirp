using Chirp.CLI;
using CSVDB;

public class Chirp_CSVDB_UnitTests
{
    [Fact]
    public void CSVDB_CsvParser_ParsingOfAuthor()
    {
        // Arrange
        string path = "../../../../../data/CsvParseTest.csv";

        // Act
        var cheeps = CSVParser.Parse<Cheep>(path);
        var cheep = cheeps.FirstOrDefault();

        // Assert
        Assert.Equal("ageh", cheep.Author);
    }
}