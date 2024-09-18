using Chirp.CLI;

public class Chirp_CLI_IntegrationTests
{
    [Theory]
    [InlineData("Michael", "I have a ball", 1690891760)]
    [InlineData("Harald", "I am him", 1726056884)]
    [InlineData("Anders", "I have Netflix", 1726056833)]
    public void PrintingCheepsTest(string author, string message, long timestamp)
    {
        // Arrange
        Cheep cheep = new Cheep(author, message, timestamp);
        List<Cheep> cheeps = new List<Cheep> { cheep };
        using (var consoleOutput = new StringWriter())
        {
            Console.SetOut(consoleOutput);


            // Act
            UserInterface.PrintCheeps(cheeps);
            string result = cheep.ToString();

            // Assert
            Assert.Equal(result, consoleOutput.ToString().Trim()); 
        }
    }
    
    /*
    [Fact]
    public void CsvToCheepInConsole() //tror den er e2e
    {
        // Arrange
        string path = "../../../../../data/CsvParseTest.csv";

        // Act
        var cheeps = CSVParser.Parse<Cheep>(path);
        using (var consoleOutput = new StringWriter())
        {
            Console.SetOut(consoleOutput);


            // Act
            UserInterface.PrintCheeps(cheeps);
            var outputLines = consoleOutput.ToString().Trim().Split(Environment.NewLine); //source: https://stackoverflow.com/a/22878533 .newLine sikrer at det virker både på mac og windows


            // Assert
            Assert.Equal("ageh @ 01/08/23 14:09:20: SIIIIIUUUUUUUU!", outputLines[0].Trim());
            Assert.Equal("nitn @ 02/08/23 14:19:38: Recently engaged", outputLines[1].Trim());
        }
    }
    */
}